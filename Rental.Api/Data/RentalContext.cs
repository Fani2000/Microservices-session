using Microsoft.EntityFrameworkCore;
using Rental.Api.Models;

namespace Rental.Api.Data;

public class RentalContext : DbContext
{
    public RentalContext(DbContextOptions<RentalContext> options)
        : base(options)
    {
    }

    public DbSet<Models.Rental> Rentals => Set<Models.Rental>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure indexes
        modelBuilder.Entity<Models.Rental>()
            .HasIndex(r => r.CustomerId);

        modelBuilder.Entity<Models.Rental>()
            .HasIndex(r => r.Status);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        // Configure owned entity
        modelBuilder.Entity<Models.Rental>()
            .OwnsOne(r => r.Book);
    }
}