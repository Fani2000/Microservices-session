using Catalog.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Workers;

public class DatabaseMigrationWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<DatabaseMigrationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Database Migration Worker starting...");

        // Add a delay to ensure the database server is ready
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        try
        {
            using var scope = scopeFactory.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationContext>>();

            using var dbContext = await dbFactory.CreateDbContextAsync(stoppingToken);

            logger.LogInformation("Applying database migrations...");
            await dbContext.Database.EnsureCreatedAsync(stoppingToken);

            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations");
        }
    }
}