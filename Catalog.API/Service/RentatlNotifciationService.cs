using BookCatalog.APIS.Protos;
using Grpc.Net.Client;

namespace Catalog.API.Services;

public class RentalNotificationService
{
    private readonly ILogger<RentalNotificationService> _logger;
    private readonly string _rentalServiceUrl;

    public RentalNotificationService(IConfiguration configuration, ILogger<RentalNotificationService> logger)
    {
        _logger = logger;
        _rentalServiceUrl = configuration["RentalService:GrpcUrl"] ?? "https://localhost:7002";
    }

    public async Task NotifyBookCreatedAsync(Guid bookId, string title, string author, string isbn)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_rentalServiceUrl);
            var client = new BookService.BookServiceClient(channel);

            var request = new BookCreatedRequest
            {
                BookId = bookId.ToString(),
                Title = title,
                Author = author,
                Isbn = isbn
            };

            var response = await client.NotifyNewBookAsync(request);

            _logger.LogInformation(
                "Book notification sent to Rental.API. Success: {Success}, Message: {Message}",
                response.Success,
                response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying Rental.API about new book {Title}", title);
        }
    }
}