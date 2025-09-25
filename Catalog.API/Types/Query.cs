using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Types;

[QueryType]
public class Query
{
    // Book Queries
    [GraphQLDescription("Get all books in the catalog")]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks([Service] ApplicationContext dbContext)
    {
        return dbContext.Books;    
    }

    [GraphQLDescription("Get a book by its ID")]
    public async Task<Book?> GetBookById(Guid id, [Service] ApplicationContext dbContext)
    {
        return await dbContext.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.ID == id);
    }
    
    [GraphQLDescription("Get books by genre")]
    public IQueryable<Book> GetBooksByGenre(string genre, [Service] ApplicationContext dbContext)
    {
        return dbContext.Books
            .Include(b => b.Author)
            .Where(b => b.Genre.ToLower() == genre.ToLower())
            .AsQueryable();
    }
    
    [GraphQLDescription("Search books by title")]
    public IQueryable<Book> SearchBooks(string searchTerm, [Service] ApplicationContext dbContext)
    {
        return dbContext.Books
            .Include(b => b.Author)
            .Where(b => b.Title.Contains(searchTerm))
            .AsQueryable();
    }
    
    // Author Queries
    [GraphQLDescription("Get all authors")]
    public IQueryable<Author> GetAuthors([Service] ApplicationContext dbContext)
    {
        return dbContext.Authors.AsQueryable();
    }
    
    [GraphQLDescription("Get an author by ID")]
    public async Task<Author?> GetAuthorById(Guid id, [Service] ApplicationContext dbContext)
    {
        return await dbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.ID == id);
    }
    
    [GraphQLDescription("Get authors with their books")]
    public IQueryable<Author> GetAuthorsWithBooks([Service] ApplicationContext dbContext)
    {
        return dbContext.Authors.Include(a => a.Books).AsQueryable();
    }
}