using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using PersonsApi.Core;
using Xunit;

/// <summary>
/// End-to-end tests running the real application (including Program.cs and DI wiring)
/// against the shipped <c>sample-input.csv</c>.
/// </summary>
public class PersonsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PersonsApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Verifies that the CSV is found and loaded when the app starts from its real configuration.
    /// </summary>
    [Fact]
    public async Task GetAll_ShouldReturnPersonsFromSampleCsv()
    {
        var persons = await _client.GetFromJsonAsync<List<Person>>("/persons");

        Assert.NotNull(persons);
        Assert.NotEmpty(persons!);
        Assert.Contains(persons, p => p.Lastname == "Petersen" && p.Color == "grün");
    }

    /// <summary>
    /// Verifies lookup by id (CSV line number).
    /// </summary>
    [Fact]
    public async Task GetById_ShouldReturnFirstCsvLine()
    {
        var person = await _client.GetFromJsonAsync<Person>("/persons/1");

        Assert.Equal("Hans", person!.Name);
        Assert.Equal("blau", person.Color);
    }

    /// <summary>
    /// Verifies that an unknown id yields 404.
    /// </summary>
    [Fact]
    public async Task GetById_Unknown_ShouldReturn404()
    {
        var response = await _client.GetAsync("/persons/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

/// <summary>
/// Startup behaviour when the configured CSV file is missing.
/// </summary>
public class PersonsApiStartupTests
{
    /// <summary>
    /// Verifies that the app refuses to start (instead of serving an empty list) when the CSV is missing,
    /// and that the error message names the offending path.
    /// </summary>
    [Fact]
    public void Startup_MissingCsv_ShouldFailFastWithClearMessage()
    {
        const string missing = "Data/does-not-exist.csv";
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseSetting("PersonsCsv:Path", missing));

        var ex = Assert.Throws<FileNotFoundException>(() => factory.CreateClient());

        Assert.Contains("does-not-exist.csv", ex.Message);
        Assert.Contains("PersonsCsv:Path", ex.Message);
    }

    /// <summary>
    /// Verifies that the CSV path can be overridden via configuration.
    /// </summary>
    [Fact]
    public async Task Startup_ConfiguredCsvPath_ShouldBeUsed()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseSetting("PersonsCsv:Path", Path.Combine("TestData", "persons.csv")));
        var client = factory.CreateClient();

        var persons = await client.GetFromJsonAsync<List<Person>>("/persons");

        Assert.NotEmpty(persons!);
    }
}
