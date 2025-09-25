namespace Rental.Api.GraphQL.Types.Inputs;

public record CreateRentalInput(
    string CustomerId,
    Guid BookId,
    DateTime DueDate,
    string? Notes);