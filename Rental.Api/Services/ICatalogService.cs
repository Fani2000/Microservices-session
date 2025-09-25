using Rental.Api.Models;

namespace Rental.Api.Services;

public interface ICatalogService
{
    Task<BookReference?> GetBookAsync(Guid bookId);
    Task<bool> IsBookAvailableAsync(Guid bookId);
}