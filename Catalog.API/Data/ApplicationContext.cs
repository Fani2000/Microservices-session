using Microsoft.EntityFrameworkCore;
using Catalog.API.Models;

namespace Catalog.API.Data
{
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
    {
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Author entity
            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Authors");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Biography).HasMaxLength(500);
                entity.Property(e => e.DateOfBirth).HasColumnType("timestamp with time zone");
                entity.Property(e => e.Nationality).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);

                // Define the relationship from Author side
                entity.HasMany(a => a.Books)
                    .WithOne(b => b.Author)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ISBN).HasMaxLength(100);
                entity.Property(e => e.PublishedDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Genre).HasMaxLength(50);
                entity.Property(e => e.AuthorId).IsRequired();
            });
        }
    }
}