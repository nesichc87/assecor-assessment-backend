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
