using Catalog.API.Data;
using Catalog.API.Models;
using Catalog.API.Services;
using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Types;

public record AddBookInput(
    string title,
    Guid authorId,
    string description = "",
    string isbn = "",
    DateTime? publishedDate = null,
    decimal price = 0,
    string genre = "",
    int pageCount = 0);

[MutationType]
public class MutationType
{
    // Book Mutations
    [GraphQLDescription("Add a new book to the catalog")]
    public async Task<Book> AddBook(
        [Service] ApplicationContext dbContext,
        [Service] ITopicEventSender eventSender,
        [Service] RentalNotificationService _rentalNotificationService,
        AddBookInput input
        )
    {
        // Verify author exists
        var author = await dbContext.Authors.FindAsync(input.authorId);
        if (author == null)
        {
            throw new GraphQLException(new Error("Author not found", "AUTHOR_NOT_FOUND"));
        }

        Models.Book book = new()
        {
            ID = Guid.NewGuid(),
            Title = input.title,
            AuthorId = input.authorId,
            Description = input.description,
            ISBN = input.isbn,
            PublishedDate = input.publishedDate ?? DateTime.UtcNow,
            Price = input.price,
            Genre = input.genre,
            PageCount = input.pageCount
        };

        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();

        await _rentalNotificationService.NotifyBookCreatedAsync(
            book.ID,
            book.Title,
            book.AuthorId.ToString(),
            book.ISBN);
        
        // Send subscription event
        await eventSender.SendAsync(nameof(Subscription.BookAdded), book);

        return book;
    }

    [GraphQLDescription("Update an existing book")]
    public async Task<Book> UpdateBook(
        [Service] ApplicationContext dbContext,
        [Service] ITopicEventSender eventSender,
        Guid id,
        string? title = null,
        Guid? authorId = null,
        string? description = null,
        string? isbn = null,
        DateTime? publishedDate = null,
        decimal? price = null,
        string? genre = null,
        int? pageCount = null)
    {
        var book = await dbContext.Books.FindAsync(id);
        if (book == null)
        {
            throw new GraphQLException(new Error("Book not found", "BOOK_NOT_FOUND"));
        }

        if (authorId.HasValue)
        {
            var author = await dbContext.Authors.FindAsync(authorId.Value);
            if (author == null)
            {
                throw new GraphQLException(new Error("Author not found", "AUTHOR_NOT_FOUND"));
            }

            book.AuthorId = authorId.Value;
        }

        if (title != null) book.Title = title;
        if (description != null) book.Description = description;
        if (isbn != null) book.ISBN = isbn;
        if (publishedDate.HasValue) book.PublishedDate = publishedDate.Value;
        if (price.HasValue) book.Price = price.Value;
        if (genre != null) book.Genre = genre;
        if (pageCount.HasValue) book.PageCount = pageCount.Value;

        dbContext.Books.Update(book);
        await dbContext.SaveChangesAsync();

        // Send subscription event
        await eventSender.SendAsync(nameof(Subscription.BookUpdated), book);

        return book;
    }

    [GraphQLDescription("Delete a book from the catalog")]
    public async Task<bool> DeleteBook(
        [Service] ApplicationContext dbContext,
        [Service] ITopicEventSender eventSender,
        Guid id)
    {
        var book = await dbContext.Books.FindAsync(id);
        if (book == null)
        {
            throw new GraphQLException(new Error("Book not found", "BOOK_NOT_FOUND"));
        }

        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync();

        // Send subscription event
        await eventSender.SendAsync(nameof(Subscription.BookDeleted), id);

        return true;
    }

    // Author Mutations
    [GraphQLDescription("Add a new author")]
    public async Task<Author> AddAuthor(
        [Service] ApplicationContext dbContext,
        [Service] ITopicEventSender eventSender,
        string name,
        string biography = "",
        DateTime? dateOfBirth = null,
        string nationality = "",
        string email = "")
    {
        Author author = new()
        {
            ID = Guid.NewGuid(),
            Name = name,
            Biography = biography,
            DateOfBirth = dateOfBirth ?? DateTime.UtcNow,
            Nationality = nationality,
            Email = email
        };

        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        // Send subscription event
        await eventSender.SendAsync(nameof(Subscription.AuthorAdded), author);

        return author;
    }

    [GraphQLDescription("Update an existing author")]
    public async Task<Author> UpdateAuthor(
        [Service] ApplicationContext dbContext,
        [Service] ITopicEventSender eventSender,
        Guid id,
        string? name = null,
        string? biography = null,
        DateTime? dateOfBirth = null,
        string? nationality = null,
        string? email = null)
    {
        var author = await dbContext.Authors.FindAsync(id);
        if (author == null)
        {
            throw new GraphQLException(new Error("Author not found", "AUTHOR_NOT_FOUND"));
        }

        if (name != null) author.Name = name;
        if (biography != null) author.Biography = biography;
        if (dateOfBirth.HasValue) author.DateOfBirth = dateOfBirth.Value;
        if (nationality != null) author.Nationality = nationality;
        if (email != null) author.Email = email;

        dbContext.Authors.Update(author);
        await dbContext.SaveChangesAsync();

        // Send subscription event
        await eventSender.SendAsync(nameof(Subscription.AuthorUpdated), author);

        return author;
    }

    [GraphQLDescription("Delete an author and all their books")]
    public async Task<bool> DeleteAuthor(
        [Service] ApplicationContext dbContext,
        [Service] ITopicEventSender eventSender,
        Guid id)
    {
        var author = await dbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.ID == id);

        if (author == null)
        {
            throw new GraphQLException(new Error("Author not found", "AUTHOR_NOT_FOUND"));
        }

        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync();

        // Send subscription event
        await eventSender.SendAsync(nameof(Subscription.AuthorDeleted), id);

        return true;
    }
}