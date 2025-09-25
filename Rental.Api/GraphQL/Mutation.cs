using HotChocolate;
using HotChocolate.Subscriptions;
using Rental.Api.Models;
using Rental.Api.Repositories;
using Rental.Api.Services;

namespace Rental.Api.GraphQL;

[MutationType]
public class Mutation
{
    public async Task<Models.Rental> CreateRental(
        [Service] IRentalRepository rentalRepository,
        [Service] ICustomerRepository customerRepository,
        [Service] ICatalogService catalogService,
        [Service] ITopicEventSender eventSender,
        CreateRentalInput input)
    {
        // Check if book is available
        var isAvailable = await catalogService.IsBookAvailableAsync(input.BookId);
        if (!isAvailable)
        {
            throw new GraphQLException(new Error("Book is not available for rental", "BOOK_UNAVAILABLE"));
        }

        // Get customer
        var customer = await customerRepository.GetByIdAsync(input.CustomerId);
        if (customer == null)
        {
            throw new GraphQLException(new Error("Customer not found", "CUSTOMER_NOT_FOUND"));
        }

        // Get book details
        var book = await catalogService.GetBookAsync(input.BookId);
        if (book == null)
        {
            throw new GraphQLException(new Error("Book not found", "BOOK_NOT_FOUND"));
        }

        // Create rental
        var rental = new Models.Rental
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            CustomerName = customer.Name,
            Book = book,
            RentalDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14), // 2 weeks rental period
            Status = "Active",
            LateFee = 0
        };

        var createdRental = await rentalRepository.CreateAsync(rental);

        // Send subscription event
        await eventSender.SendAsync("RentalCreated", createdRental);

        return createdRental;
    }

    public async Task<Models.Rental?> ReturnRental(
        [Service] IRentalRepository rentalRepository,
        [Service] ITopicEventSender eventSender,
        Guid id)
    {
        var rental = await rentalRepository.ReturnBookAsync(id);

        if (rental != null)
        {
            await eventSender.SendAsync("RentalReturned", rental);
        }

        return rental;
    }

    public async Task<Customer> CreateCustomer(
        [Service] ICustomerRepository customerRepository,
        [Service] ITopicEventSender eventSender,
        CreateCustomerInput input)
    {
        // Check if email is already in use
        var existingCustomer = await customerRepository.GetByEmailAsync(input.Email);
        if (existingCustomer != null)
        {
            throw new GraphQLException(new Error("Email already in use", "EMAIL_IN_USE"));
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = input.Name,
            Email = input.Email,
            Phone = input.Phone,
            Address = input.Address
        };

        var createdCustomer = await customerRepository.CreateAsync(customer);

        await eventSender.SendAsync("CustomerCreated", createdCustomer);

        return createdCustomer;
    }

    public async Task<bool> UpdateCustomer(
        [Service] ICustomerRepository customerRepository,
        [Service] ITopicEventSender eventSender,
        UpdateCustomerInput input)
    {
        var customer = await customerRepository.GetByIdAsync(input.Id);
        if (customer == null)
        {
            throw new GraphQLException(new Error("Customer not found", "CUSTOMER_NOT_FOUND"));
        }

        // If email is changing, check if new email is already in use
        if (input.Email != customer.Email)
        {
            var existingCustomer = await customerRepository.GetByEmailAsync(input.Email);
            if (existingCustomer != null && existingCustomer.Id != input.Id)
            {
                throw new GraphQLException(new Error("Email already in use", "EMAIL_IN_USE"));
            }
        }

        customer.Name = input.Name;
        customer.Email = input.Email;
        customer.Phone = input.Phone;
        customer.Address = input.Address;

        var success = await customerRepository.UpdateAsync(customer);

        if (success)
        {
            await eventSender.SendAsync("CustomerUpdated", customer);
        }

        return success;
    }

    public async Task<bool> DeleteCustomer(
        [Service] ICustomerRepository customerRepository,
        [Service] ITopicEventSender eventSender,
        Guid id)
    {
        var success = await customerRepository.DeleteAsync(id);

        if (success)
        {
            await eventSender.SendAsync("CustomerDeleted", id);
        }

        return success;
    }
}

// Input types
public record CreateRentalInput(Guid CustomerId, Guid BookId);

public record CreateCustomerInput(string Name, string Email, string Phone, string Address);

public record UpdateCustomerInput(Guid Id, string Name, string Email, string Phone, string Address);