using Rental.Api.Models;

namespace Rental.Api.Repositories;

public interface IRentalRepository
{
    Task<IEnumerable<Models.Rental>> GetAllAsync();
    Task<Models.Rental?> GetByIdAsync(Guid id);
    Task<IEnumerable<Models.Rental>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Models.Rental>> GetActiveRentalsAsync();
    Task<IEnumerable<Models.Rental>> GetOverdueRentalsAsync();
    Task<Models.Rental> CreateAsync(Models.Rental rental);
    Task<bool> UpdateAsync(Models.Rental rental);
    Task<bool> DeleteAsync(Guid id);
    Task<Models.Rental?> ReturnBookAsync(Guid id);
}