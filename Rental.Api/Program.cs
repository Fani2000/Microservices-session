using Microsoft.EntityFrameworkCore;
using Rental.Api.Data;
using Rental.Api.Extensions;
using Rental.Api.GraphQL.Types;
using Rental.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (for Aspire)
builder.AddServiceDefaults();

// Add PostgreSQL instead of MongoDB
builder.Services.AddPostgresDatabase(builder.Configuration);

// Add services
builder.Services.AddServices();
builder.Services.AddGrpc();

// Add GraphQL
builder
    .AddGraphQL()
    .AddTypes()
    .AddType<RentalType>()
    .AddType<CustomerType>()
    .AddType<BookReferenceType>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddQueryConventions()
    .AddMutationConventions()
    .AddInMemorySubscriptions();

builder.Services.AddTypes();

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Initializing and migrating rental database...");
        
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<RentalContext>>();
        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while initializing the database: {Message}", ex.Message);
}

app.MapGrpcService<BookGrpcService>();

// Add Aspire service defaults middleware
app.MapDefaultEndpoints();

app.MapGraphQLWebSocket();
app.MapGraphQL();

app.RunWithGraphQLCommands(args);
