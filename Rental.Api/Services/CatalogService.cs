using System.Net.Http.Json;
using Rental.Api.Models;

namespace Rental.Api.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(IHttpClientFactory httpClientFactory, ILogger<CatalogService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("catalog-api");
        _logger = logger;
    }

    public async Task<BookReference?> GetBookAsync(Guid bookId)
    {
        try
        {
            var bookDto = await _httpClient.GetFromJsonAsync<BookDto>($"/api/books/{bookId}");

            if (bookDto == null) return null;

            return new BookReference
            {
                BookId = bookDto.ID,
                Title = bookDto.Title,
                ISBN = bookDto.ISBN,
                Author = bookDto.Author?.Name ?? "Unknown Author"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching book {BookId} from Catalog API", bookId);
            return null;
        }
    }

    public async Task<bool> IsBookAvailableAsync(Guid bookId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/books/{bookId}/availability");

            if (response.IsSuccessStatusCode)
            {
                var availability = await response.Content.ReadFromJsonAsync<BookAvailabilityDto>();
                return availability?.IsAvailable ?? false;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking book availability for {BookId}", bookId);
            return false;
        }
    }

    // DTOs for deserializing catalog API responses
    private class BookDto
    {
        public Guid ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public AuthorDto? Author { get; set; }
    }

    private class AuthorDto
    {
        public Guid ID { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class BookAvailabilityDto
    {
        public Guid BookId { get; set; }
        public bool IsAvailable { get; set; }
        public int AvailableCopies { get; set; }
    }
}