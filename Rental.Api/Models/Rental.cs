using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rental.Api.Models;

public class Rental
{
    [Key] public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public BookReference Book { get; set; } = new();

    public DateTime RentalDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal LateFee { get; set; }

    public string? Notes { get; set; }

    // Navigation property
    [ForeignKey("CustomerId")] public Customer? Customer { get; set; }
}
