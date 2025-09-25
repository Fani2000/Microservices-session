using BookCatalog.APIS.Protos;
using Microsoft.EntityFrameworkCore;
using Rental.Api.Data;
using Rental.Api.Models;
using Grpc.Core;

namespace Rental.Api.Services;

public class BookGrpcService : BookService.BookServiceBase
{
    private readonly IDbContextFactory<RentalContext> _contextFactory;
    private readonly ILogger<BookGrpcService> _logger;

    public BookGrpcService(IDbContextFactory<RentalContext> contextFactory, ILogger<BookGrpcService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public override async Task<BookCreatedResponse> NotifyNewBook(BookCreatedRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Received new book notification: {Title}", request.Title);

            // Create a BookReference from the request
            var bookReference = new BookReference
            {
                BookId = Guid.Parse(request.BookId),
                Title = request.Title,
                Author = request.Author,
                ISBN = request.Isbn
            };

            _logger.LogInformation("Book registered in Rental.API: {Id}, {Title}, {Author}, {ISBN}",
                bookReference.BookId, bookReference.Title, bookReference.Author, bookReference.ISBN);

            return new BookCreatedResponse
            {
                Success = true,
                Message = $"Book '{request.Title}' successfully registered in Rental.API"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing book notification");
            return new BookCreatedResponse
            {
                Success = false,
                Message = $"Failed to register book: {ex.Message}"
            };
        }
    }
}