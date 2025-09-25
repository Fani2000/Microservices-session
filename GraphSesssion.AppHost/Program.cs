var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("catalog-db")
    .AddDatabase("catalogdb");

// Create a new Postgres instance for the rental API
var rentalPostgres = builder.AddPostgres("rental-db")
    .AddDatabase("RentalDB");



// Add Catalog API with SQLite reference and explicit connection string mapping
var catalogApi = builder.AddProject<Projects.Catalog_API>("catalog-api")
    .WithReference(postgres) 
    .WaitFor(postgres)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development"); // Ensure development mode for database seeding

var rentalApi = builder.AddProject<Projects.Rental_APi>("rental-api")
    .WithReference(rentalPostgres)
    .WithReference(catalogApi) // Reference to Catalog API for book information
    .WaitFor(rentalPostgres)
    .WaitFor(catalogApi)
    .WithEnvironment("GraphQL__EnableSchemaExport", "true")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development");

builder
    .AddFusionGateway<Projects.Gateway>("gateway")
    .WithSubgraph(catalogApi)
    .WithSubgraph(rentalApi);

builder.Build().Compose().Run();