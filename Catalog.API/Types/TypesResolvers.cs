using Catalog.API.Data;
using Catalog.API.Models;

namespace Catalog.API.Types;

public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor.Description("Represents a book in the catalog");

        descriptor.Field(b => b.ID)
            .Description("The unique identifier of the book");

        descriptor.Field(b => b.Title)
            .Description("The title of the book");

        descriptor.Field(b => b.Description)
            .Description("A description or summary of the book");

        descriptor.Field(b => b.ISBN)
            .Description("The International Standard Book Number");

        descriptor.Field(b => b.PublishedDate)
            .Description("The date when the book was published");

        descriptor.Field(b => b.Price)
            .Description("The price of the book");

        descriptor.Field(b => b.Genre)
            .Description("The genre or category of the book");

        descriptor.Field(b => b.PageCount)
            .Description("The total number of pages in the book");

        descriptor.Field(b => b.AuthorId)
            .Description("The ID of the author who wrote this book");

        descriptor.Field(b => b.Author)
            .Ignore()
            .Description("The author who wrote this book")
            .ResolveWith<BookResolvers>(r => r.GetAuthor(default!, default!));
    }

    private class BookResolvers
    {
        public Author GetAuthor(Book book, [Service] ApplicationContext dbContext)
        {
            return dbContext.Authors.FirstOrDefault(a => a.ID == book.AuthorId)
                   ?? throw new GraphQLException(new Error("Author not found", "AUTHOR_NOT_FOUND"));
        }
    }
}

public class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {
        descriptor.Description("Represents an author of books");

        descriptor.Field(a => a.ID)
            .Description("The unique identifier of the author");

        descriptor.Field(a => a.Name)
            .Description("The full name of the author");

        descriptor.Field(a => a.Biography)
            .Description("A biographical summary of the author");

        descriptor.Field(a => a.DateOfBirth)
            .Description("The author's date of birth");

        descriptor.Field(a => a.Nationality)
            .Description("The nationality of the author");

        descriptor.Field(a => a.Email)
            .Description("The author's contact email");

        descriptor.Field(a => a.Books)
            .Description("The books written by this author")
            .ResolveWith<AuthorResolvers>(r => r.GetBooks(default!, default!));
    }

    private class AuthorResolvers
    {
        public IEnumerable<Book> GetBooks(Author author, [Service] ApplicationContext dbContext)
        {
            return dbContext.Books.Where(b => b.AuthorId == author.ID).ToList();
        }
    }
}