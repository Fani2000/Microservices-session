using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using Rental.Api.Models;

namespace Rental.Api.GraphQL;

[SubscriptionType]
public class Subscription
{
    [Subscribe]
    [Topic("RentalCreated")]
    public Models.Rental OnRentalCreated([EventMessage] Models.Rental rental)
    {
        return rental;
    }

    [Subscribe]
    [Topic("RentalReturned")]
    public Models.Rental OnRentalReturned([EventMessage] Models.Rental rental)
    {
        return rental;
    }

    [Subscribe]
    [Topic("CustomerCreated")]
    public Customer OnCustomerCreated([EventMessage] Customer customer)
    {
        return customer;
    }

    [Subscribe]
    [Topic("CustomerUpdated")]
    public Customer OnCustomerUpdated([EventMessage] Customer customer)
    {
        return customer;
    }

    [Subscribe]
    [Topic("CustomerDeleted")]
    public Guid OnCustomerDeleted([EventMessage] Guid customerId)
    {
        return customerId;
    }
}