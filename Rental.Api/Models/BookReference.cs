using Microsoft.EntityFrameworkCore;

namespace Rental.Api.Models;

[Owned]
public class BookReference
{
    public Guid BookId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string Author { get; set; } = string.Empty;
    
    public string ISBN { get; set; } = string.Empty;
}