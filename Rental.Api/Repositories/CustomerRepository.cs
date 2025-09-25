using Microsoft.EntityFrameworkCore;
using Rental.Api.Data;
using Rental.Api.Models;

namespace Rental.Api.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly RentalContext _context;

    public CustomerRepository(RentalContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .Include(c => c.Rentals)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers
            .Include(c => c.Rentals)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .Include(c => c.Rentals)
            .FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // Check if customer has active rentals
        var customer = await _context.Customers
            .Include(c => c.Rentals.Where(r => r.Status == "Active"))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
            return false;

        if (customer.Rentals.Any())
        {
            throw new InvalidOperationException("Cannot delete customer with active rentals");
        }

        _context.Customers.Remove(customer);
        return await _context.SaveChangesAsync() > 0;
    }
}