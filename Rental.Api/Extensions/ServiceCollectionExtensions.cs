using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Rental.Api.Data;
using Rental.Api.GraphQL;
using Rental.Api.GraphQL.Types;
using Rental.Api.Repositories;
using Rental.Api.Services;

namespace Rental.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTypes(this IServiceCollection services)
    {
        // Register GraphQL types
        services.AddSingleton<RentalType>();
        services.AddSingleton<CustomerType>();
        services.AddSingleton<BookReferenceType>();

        return services;
    }

    public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RentalDB");
        Console.WriteLine($"Using rental database connection string: {connectionString}");

        services.AddDbContextFactory<RentalContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions => 
            {
                npgsqlOptions.MigrationsAssembly("Rental.Api");
                // Add retry logic for connection resilience
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            }));

        // Register repositories
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Use Aspire's typed client for the Catalog API
        services.AddHttpClient<ICatalogService, CatalogService>("services__catalog-api__http__0");

        return services;
    }
}