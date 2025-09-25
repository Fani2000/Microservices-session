using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data;

public class DatabaseSeeder(
    IDbContextFactory<ApplicationContext> dbContextFactory,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting database seeding process...");

        using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        try
        {
            // Check if database already has data
            bool hasAuthors = await dbContext.Authors.AnyAsync(cancellationToken);

            if (hasAuthors)
            {
                logger.LogInformation("Database already contains data, skipping seed");
                return;
            }

            logger.LogInformation("Seeding database with initial data...");

            // Create authors
            var authors = new List<Author>
            {
                new Author
                {
                    ID = Guid.NewGuid(),
                    Name = "J.K. Rowling",
                    Biography = "British author known for the Harry Potter series",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1965, 7, 31), DateTimeKind.Utc),
                    Nationality = "British",
                    Email = "jkrowling@example.com"
                },
                new Author
                {
                    ID = Guid.NewGuid(),
                    Name = "George R.R. Martin",
                    Biography = "American novelist and short story writer, screenwriter, and television producer",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1948, 9, 20), DateTimeKind.Utc),
                    Nationality = "American",
                    Email = "grrmartin@example.com"
                },
                new Author
                {
                    ID = Guid.NewGuid(),
                    Name = "J.R.R. Tolkien",
                    Biography = "English writer, poet, philologist, and academic",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1892, 1, 3), DateTimeKind.Utc),
                    Nationality = "British",
                    Email = "jrrtolkien@example.com"
                },
                new Author
                {
                    ID = Guid.NewGuid(),
                    Name = "Agatha Christie",
                    Biography = "English writer known for her detective novels",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1890, 9, 15), DateTimeKind.Utc),
                    Nationality = "British",
                    Email = "achristie@example.com"
                },
                new Author
                {
                    ID = Guid.NewGuid(),
                    Name = "Stephen King",
                    Biography = "American author of horror, supernatural fiction, suspense, and fantasy novels",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1947, 9, 21), DateTimeKind.Utc),
                    Nationality = "American",
                    Email = "sking@example.com"
                }
            };

            await dbContext.Authors.AddRangeAsync(authors, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added {Count} authors to database", authors.Count);

            // Create books with our enhanced properties and UTC DateTime values
            var books = new List<Book>
            {
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "Harry Potter and the Philosopher's Stone",
                    Description = "Harry Potter discovers his magical heritage on his 11th birthday",
                    ISBN = "978-0747532743",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1997, 6, 26), DateTimeKind.Utc),
                    Price = 19.99m,
                    Genre = "Fantasy",
                    PageCount = 223,
                    AuthorId = authors[0].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "Harry Potter and the Chamber of Secrets",
                    Description = "Harry's second year at Hogwarts School of Witchcraft and Wizardry",
                    ISBN = "978-0747538486",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1998, 7, 2), DateTimeKind.Utc),
                    Price = 19.99m,
                    Genre = "Fantasy",
                    PageCount = 251,
                    AuthorId = authors[0].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "A Game of Thrones",
                    Description = "First novel in the series A Song of Ice and Fire",
                    ISBN = "978-0553103540",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1996, 8, 1), DateTimeKind.Utc),
                    Price = 24.99m,
                    Genre = "Fantasy",
                    PageCount = 694,
                    AuthorId = authors[1].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "A Clash of Kings",
                    Description = "Second novel in the series A Song of Ice and Fire",
                    ISBN = "978-0553108033",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1998, 11, 16), DateTimeKind.Utc),
                    Price = 24.99m,
                    Genre = "Fantasy",
                    PageCount = 768,
                    AuthorId = authors[1].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "The Fellowship of the Ring",
                    Description = "First part of The Lord of the Rings trilogy",
                    ISBN = "978-0618346257",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1954, 7, 29), DateTimeKind.Utc),
                    Price = 22.99m,
                    Genre = "Fantasy",
                    PageCount = 423,
                    AuthorId = authors[2].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "The Two Towers",
                    Description = "Second part of The Lord of the Rings trilogy",
                    ISBN = "978-0618346264",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1954, 11, 11), DateTimeKind.Utc),
                    Price = 22.99m,
                    Genre = "Fantasy",
                    PageCount = 352,
                    AuthorId = authors[2].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "Murder on the Orient Express",
                    Description = "Detective novel featuring Hercule Poirot",
                    ISBN = "978-0062073501",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1934, 1, 1), DateTimeKind.Utc),
                    Price = 14.99m,
                    Genre = "Mystery",
                    PageCount = 256,
                    AuthorId = authors[3].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "Death on the Nile",
                    Description = "A murder mystery novel featuring Hercule Poirot",
                    ISBN = "978-0062073556",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1937, 11, 1), DateTimeKind.Utc),
                    Price = 14.99m,
                    Genre = "Mystery",
                    PageCount = 288,
                    AuthorId = authors[3].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "The Shining",
                    Description = "A horror novel about a family staying at an isolated hotel",
                    ISBN = "978-0307743657",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1977, 1, 28), DateTimeKind.Utc),
                    Price = 17.99m,
                    Genre = "Horror",
                    PageCount = 447,
                    AuthorId = authors[4].ID
                },
                new Book
                {
                    ID = Guid.NewGuid(),
                    Title = "It",
                    Description = "Horror novel about a group of children terrorized by an evil entity",
                    ISBN = "978-1501142970",
                    PublishedDate = DateTime.SpecifyKind(new DateTime(1986, 9, 15), DateTimeKind.Utc),
                    Price = 19.99m,
                    Genre = "Horror",
                    PageCount = 1138,
                    AuthorId = authors[4].ID
                }
            };

            await dbContext.Books.AddRangeAsync(books, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added {Count} books to database", books.Count);

            logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}