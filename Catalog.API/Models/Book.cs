using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models;

[GraphQLDescription("Represents a book.")]
public class Book
{
    [Key] public Guid ID { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [StringLength(100)]
    [GraphQLDescription("ISBN number of the book.")]
    public string ISBN { get; set; } = string.Empty;
    
    public DateTime PublishedDate { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    [StringLength(50)]
    public string Genre { get; set; } = string.Empty;
    
    public int PageCount { get; set; }
    
    // Match the case of AuthorId to match what's in the ApplicationContext
    [Column("AuthorId")] 
    public Guid AuthorId { get; set; }

    [ForeignKey("AuthorId")] 
    public Author? Author { get; set; }
}