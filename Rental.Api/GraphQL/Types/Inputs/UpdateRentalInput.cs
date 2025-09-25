namespace Rental.Api.GraphQL.Types.Inputs;

public record UpdateRentalInput(
    string? Status,
    DateTime? DueDate,
    string? Notes);