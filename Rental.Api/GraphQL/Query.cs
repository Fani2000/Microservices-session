using HotChocolate;
using HotChocolate.Data;
using Rental.Api.Data;
using Rental.Api.Models;
using Rental.Api.Repositories;
using Rental.Api.Services;

namespace Rental.Api.GraphQL;

[QueryType]
public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Models.Rental> GetRentals([Service] RentalContext context)
    {
        return context.Rentals;
    }

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers([Service] RentalContext context)
    {
        return context.Customers;
    }

    public async Task<Models.Rental?> GetRentalById(
        [Service] IRentalRepository repository,
        Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<Customer?> GetCustomerById(
        [Service] ICustomerRepository repository,
        Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<Customer?> GetCustomerByEmail(
        [Service] ICustomerRepository repository,
        string email)
    {
        return await repository.GetByEmailAsync(email);
    }

    public async Task<IEnumerable<Models.Rental>> GetActiveRentals(
        [Service] IRentalRepository repository)
    {
        return await repository.GetActiveRentalsAsync();
    }

    public async Task<IEnumerable<Models.Rental>> GetOverdueRentals(
        [Service] IRentalRepository repository)
    {
        return await repository.GetOverdueRentalsAsync();
    }

    public async Task<bool> IsBookAvailable(
        [Service] ICatalogService catalogService,
        Guid bookId)
    {
        return await catalogService.IsBookAvailableAsync(bookId);
    }
}