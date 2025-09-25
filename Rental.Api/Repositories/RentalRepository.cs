using Microsoft.EntityFrameworkCore;
using Rental.Api.Data;
using Rental.Api.Models;

namespace Rental.Api.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly RentalContext _context;

    public RentalRepository(RentalContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Models.Rental>> GetAllAsync()
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .ToListAsync();
    }

    public async Task<Models.Rental?> GetByIdAsync(Guid id)
    {
        return await _context.Rentals
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Models.Rental>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.Rentals
            .Where(r => r.CustomerId == customerId)
            .Include(r => r.Customer)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Rental>> GetActiveRentalsAsync()
    {
        return await _context.Rentals
            .Where(r => r.Status == "Active")
            .Include(r => r.Customer)
            .ToListAsync();
    }

    public async Task<IEnumerable<Models.Rental>> GetOverdueRentalsAsync()
    {
        var today = DateTime.UtcNow.Date;

        return await _context.Rentals
            .Where(r => r.Status == "Active" && r.DueDate < today)
            .Include(r => r.Customer)
            .ToListAsync();
    }

    public async Task<Models.Rental> CreateAsync(Models.Rental rental)
    {
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();
        return rental;
    }

    public async Task<bool> UpdateAsync(Models.Rental rental)
    {
        _context.Rentals.Update(rental);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var rental = await _context.Rentals.FindAsync(id);
        if (rental == null)
            return false;

        _context.Rentals.Remove(rental);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Models.Rental?> ReturnBookAsync(Guid id)
    {
        var rental = await _context.Rentals
            .FirstOrDefaultAsync(r => r.Id == id && r.Status == "Active");

        if (rental == null)
            return null;

        rental.ReturnDate = DateTime.UtcNow;
        rental.Status = "Returned";

        // Calculate late fee if applicable
        if (rental.ReturnDate > rental.DueDate)
        {
            var daysLate = (rental.ReturnDate.Value - rental.DueDate).Days;
            rental.LateFee = daysLate * 0.50m; // $0.50 per day late
        }

        await _context.SaveChangesAsync();
        return rental;
    }
}