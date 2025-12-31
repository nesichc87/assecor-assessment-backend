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
builder.Services.AddSingleton<IPersonRepository>(
    new CsvPersonRepository("sample-input.csv")
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