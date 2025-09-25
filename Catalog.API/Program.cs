using Catalog.API.Data;
using Catalog.API.Services;
using Catalog.API.Types;
using Catalog.API.Workers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Aspire-managed database connection
builder.AddServiceDefaults();

// Configure SQLite using connection string from configuration with enhanced settings
var connectionString = builder.Configuration.GetConnectionString("catalogdb");
Console.WriteLine($"Using database connection string: {connectionString}");

builder.Services.AddDbContextFactory<ApplicationContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => 
    {
        npgsqlOptions.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName);
        // Add retry logic for connection resilience
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    }));




// Register database seeder service
builder.Services.AddScoped<DatabaseSeeder>();

// Register database migration worker as a hosted service
builder.Services.AddHostedService<DatabaseMigrationWorker>();

builder.Services.AddSingleton<RentalNotificationService>();


// GraphQL configuration
builder
    .AddGraphQL()
    .AddTypes()
    .AddType<BookType>()
    .AddType<AuthorType>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddQueryConventions()
    .AddMutationConventions()
    .AddInMemorySubscriptions();

var app = builder.Build();

// Add Aspire service defaults middleware
app.MapDefaultEndpoints();

app.MapGraphQLWebSocket();
app.MapGraphQL();

// Seed the database immediately on startup regardless of environment
try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Initializing and seeding database...");
        
        // Ensure database is created first
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationContext>>();
        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            await dbContext.Database.EnsureCreatedAsync();
        }
        
        // Then seed the database
        var databaseSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await databaseSeeder.SeedAsync();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding the database: {Message}", ex.Message);
}



app.RunWithGraphQLCommands(args);