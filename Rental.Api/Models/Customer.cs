
using System.ComponentModel.DataAnnotations;

namespace Rental.Api.Models;
public class Customer
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    public string Phone { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}

