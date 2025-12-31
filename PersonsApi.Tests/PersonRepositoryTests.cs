using System.Collections.Generic;
using System.IO;
using System.Linq;
using PersonsApi.Core;
using PersonsApi.Infrastructure;
using Xunit;

/// <summary>
/// Unit tests for the <see cref="CsvPersonRepository"/>.
/// </summary>
public class PersonRepositoryTests
{
    /// <summary>
    /// Path to the temporary CSV file used during tests.
    /// </summary>
    private const string TestCsvPath = "test_persons.csv";

    /// <summary>
    /// Creates a test CSV file without a header row.
    /// The CSV format matches the production input format.
    /// </summary>
    private void CreateTestCsv()
    {
        File.WriteAllLines(TestCsvPath, new[]
        {
            "Müller, Hans, 67742 Lauterecken, 1",
            "Schmidt, Anna, 10115 Berlin, 4"
        });
    }

    /// <summary>
    /// Verifies that all persons from the CSV file and
    /// all in-memory additions are returned.
    /// </summary>
    [Fact]
    public void GetAll_ShouldReturnAllCsvAndInMemoryPersons()
    {
        CreateTestCsv();
        var repo = new CsvPersonRepository(TestCsvPath);

        // Add an in-memory person (CSV file remains unchanged)
        repo.Add(new Person
        {
            Id = 3,
            Name = "Lisa",
            Lastname = "Meier",
            Zipcode = "12345",
            City = "Musterstadt",
            Color = "grün"
        });

        var all = repo.GetAll().ToList();

        Assert.Equal(3, all.Count);
        Assert.Contains(all, p => p.Name == "Lisa");
    }

    /// <summary>
    /// Verifies that a person can be retrieved by ID
    /// and that a non-existing ID returns null.
    /// </summary>
    [Fact]
    public void GetById_ShouldReturnCorrectPersonOrNull()
    {
        CreateTestCsv();
        var repo = new CsvPersonRepository(TestCsvPath);

        var person = repo.GetById(1);
        Assert.NotNull(person);
        Assert.Equal("Hans", person!.Name);

        var missing = repo.GetById(99);
        Assert.Null(missing);
    }

    /// <summary>
    /// Verifies that persons are correctly filtered by color.
    /// </summary>
    [Fact]
    public void GetByColor_ShouldReturnFilteredPersons()
    {
        CreateTestCsv();
        var repo = new CsvPersonRepository(TestCsvPath);

        var bluePersons = repo.GetByColor("blau").ToList();
        Assert.Single(bluePersons);
        Assert.Equal("Hans", bluePersons[0].Name);

        var none = repo.GetByColor("gelb").ToList();
        Assert.Empty(none);
    }

    /// <summary>
    /// Verifies that a missing CSV file does not throw an exception
    /// and returns an empty result.
    /// </summary>
    [Fact]
    public void GetAll_MissingCsvFile_ShouldReturnEmpty()
    {
        var repo = new CsvPersonRepository("does_not_exist.csv");

        var result = repo.GetAll().ToList();

        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that an empty CSV file results in no persons.
    /// </summary>
    [Fact]
    public void GetAll_EmptyCsvFile_ShouldReturnEmpty()
    {
        File.WriteAllText(TestCsvPath, string.Empty);
        var repo = new CsvPersonRepository(TestCsvPath);

        var result = repo.GetAll().ToList();

        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that malformed CSV lines are ignored.
    /// </summary>
    [Fact]
    public void GetAll_InvalidCsvLine_ShouldBeIgnored()
    {
        File.WriteAllLines(TestCsvPath, new[]
        {
            "Müller, Hans, 67742 Lauterecken, 1",
            "INVALID LINE WITHOUT ENOUGH DATA"
        });

        var repo = new CsvPersonRepository(TestCsvPath);

        var result = repo.GetAll().ToList();

        Assert.Single(result);
        Assert.Equal("Hans", result[0].Name);
    }

    /// <summary>
    /// Verifies that an unknown color ID is mapped to "unknown".
    /// </summary>
    [Fact]
    public void GetAll_UnknownColorId_ShouldMapToUnknown()
    {
        File.WriteAllLines(TestCsvPath, new[]
        {
            "Müller, Hans, 67742 Lauterecken, 99"
        });

        var repo = new CsvPersonRepository(TestCsvPath);

        var person = repo.GetAll().Single();

        Assert.Equal("unknown", person.Color);
    }

    /// <summary>
    /// Verifies that GetByColor handles null input gracefully.
    /// </summary>
    [Fact]
    public void GetByColor_Null_ShouldReturnEmpty()
    {
        CreateTestCsv();
        var repo = new CsvPersonRepository(TestCsvPath);

        var result = repo.GetByColor(null!).ToList();

        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that GetByColor handles empty input gracefully.
    /// </summary>
    [Fact]
    public void GetByColor_EmptyString_ShouldReturnEmpty()
    {
        CreateTestCsv();
        var repo = new CsvPersonRepository(TestCsvPath);

        var result = repo.GetByColor(string.Empty).ToList();

        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that adding a null person does not throw an exception.
    /// </summary>
    [Fact]
    public void Add_NullPerson_ShouldNotThrow()
    {
        CreateTestCsv();
        var repo = new CsvPersonRepository(TestCsvPath);

        var exception = Record.Exception(() => repo.Add(null!));

        Assert.Null(exception);
    }
}
