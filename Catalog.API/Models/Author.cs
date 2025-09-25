using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models;

public class Author
{
    [Key] public Guid ID { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Biography { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    
    [StringLength(100)]
    public string Nationality { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    // Navigation property for the relationship
    public ICollection<Book> Books { get; set; } = new List<Book>();
}