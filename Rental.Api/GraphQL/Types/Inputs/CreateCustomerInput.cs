namespace Rental.Api.GraphQL.Types.Inputs;

public record CreateCustomerInput(
    string Name,
    string Email,
    string Phone,
    string Address);