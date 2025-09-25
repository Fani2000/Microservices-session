using Rental.Api.Models;
using Rental.Api.Repositories;

namespace Rental.Api.GraphQL.Types;

public class CustomerType : ObjectType<Customer>
{
    protected override void Configure(IObjectTypeDescriptor<Customer> descriptor)
    {
        descriptor
            .Field(c => c.Id)
            .Type<NonNullType<StringType>>()
            .Description("The unique identifier of the customer");

        descriptor
            .Field(c => c.Name)
            .Type<NonNullType<StringType>>()
            .Description("The name of the customer");

        descriptor
            .Field(c => c.Email)
            .Type<NonNullType<StringType>>()
            .Description("The email address of the customer");

        descriptor
            .Field(c => c.Phone)
            .Type<NonNullType<StringType>>()
            .Description("The phone number of the customer");

        descriptor
            .Field(c => c.Address)
            .Type<NonNullType<StringType>>()
            .Description("The address of the customer");

        descriptor
            .Field("rentals")
            .ResolveWith<CustomerResolvers>(r => r.GetRentals(default!, default!))
            .Description("Get all rentals (active and history) for this customer");
    }

    private class CustomerResolvers
    {
        public async Task<IEnumerable<Models.Rental>> GetRentals(
            [Parent] Customer customer,
            [Service] IRentalRepository rentalRepository)
        {
            return await rentalRepository.GetByCustomerIdAsync(customer.Id);
        }
    }
}