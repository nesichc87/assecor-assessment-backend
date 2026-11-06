using PersonsApi.Core;
using PersonsApi.Infrastructure;
using Microsoft.OpenApi.Models;

// Creates the WebApplicationBuilder (configuration, logging, DI, hosting)
var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Dependency Injection
// ------------------------------------------------------------
// Registers the IPersonRepository as a singleton.
// Currently a CSV-based implementation is used.
// Thanks to DI, the data source can easily be replaced later
// (e.g. database, external service) without changing controllers.
// The CSV location is configurable (key "PersonsCsv:Path", e.g. via appsettings or the
// PersonsCsv__Path environment variable). Relative paths are resolved against the
// application's base directory, not the current working directory.
var configuredPath = builder.Configuration["PersonsCsv:Path"];
var csvPath = Path.Combine(
    AppContext.BaseDirectory,
    string.IsNullOrWhiteSpace(configuredPath) ? Path.Combine("Data", "sample-input.csv") : configuredPath);

// Fail fast: a missing CSV would otherwise silently yield an empty API.
if (!File.Exists(csvPath))
{
    throw new FileNotFoundException(
        $"Person data CSV not found at '{csvPath}'. Set 'PersonsCsv:Path' (relative to '{AppContext.BaseDirectory}' or absolute) or make sure Data/sample-input.csv is copied to the output directory.",
        csvPath);
}

builder.Services.AddSingleton<IPersonRepository>(sp =>
    new CsvPersonRepository(csvPath, sp.GetRequiredService<ILogger<CsvPersonRepository>>())
);

// ------------------------------------------------------------
// MVC / Controller support
// ------------------------------------------------------------
builder.Services.AddControllers();

// ------------------------------------------------------------
// Swagger / OpenAPI configuration
// ------------------------------------------------------------
// Required for Swagger endpoint discovery
builder.Services.AddEndpointsApiExplorer();

// Configures Swagger documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Persons API",
        Version = "v1"
    });
});

// Builds the WebApplication
var app = builder.Build();

// ------------------------------------------------------------
// HTTP request pipeline
// ------------------------------------------------------------

// Enable Swagger only in Development environment
// (best practice for security and CI/CD pipelines)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Persons API v1"));
}

// Enforces HTTPS
app.UseHttpsRedirection();

// Authorization middleware
// (no authentication configured yet, but ready for extension)
app.UseAuthorization();

// Maps controller endpoints (REST API)
app.MapControllers();

// Starts the application
app.Run();

/// <summary>
/// Application entry point; declared public so integration tests can use WebApplicationFactory.
/// </summary>
public partial class Program { }