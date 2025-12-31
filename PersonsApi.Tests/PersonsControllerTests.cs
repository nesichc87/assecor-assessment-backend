using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PersonsApi.Controllers;
using PersonsApi.Core;
using Xunit;

/// <summary>
/// Unit tests for <see cref="PersonsController"/>.
/// </summary>
public class PersonsControllerTests
{
    /// <summary>
    /// Verifies that GET /persons returns HTTP 200 (OK)
    /// with a list of persons.
    /// </summary>
    [Fact]
    public void GetAll_ShouldReturnOkWithPersons()
    {
        // Arrange
        var mockRepo = new Mock<IPersonRepository>();
        mockRepo.Setup(r => r.GetAll()).Returns(new List<Person>
        {
            new Person
            {
                Id = 1,
                Name = "Hans",
                Lastname = "Müller",
                Zipcode = "67742",
                City = "Lauterecken",
                Color = "blau"
            }
        });

        var controller = new PersonsController(mockRepo.Object);

        // Act
        var result = controller.GetAll() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var persons = Assert.IsAssignableFrom<IEnumerable<Person>>(result!.Value);
        Assert.Single(persons);
    }

    /// <summary>
    /// Verifies that GET /persons/{id} returns HTTP 404 (Not Found)
    /// when the requested person does not exist.
    /// </summary>
    [Fact]
    public void GetById_ShouldReturnNotFound_WhenPersonMissing()
    {
        // Arrange
        var mockRepo = new Mock<IPersonRepository>();
        mockRepo
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns((Person?)null);

        var controller = new PersonsController(mockRepo.Object);

        // Act
        var result = controller.GetById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    /// Verifies that POST /persons returns HTTP 201 (Created)
    /// and returns the created person.
    /// </summary>
    [Fact]
    public void AddPerson_ShouldReturnCreatedPerson()
    {
        // Arrange
        var mockRepo = new Mock<IPersonRepository>();
        mockRepo.Setup(r => r.GetAll()).Returns(new List<Person>());
        mockRepo.Setup(r => r.Add(It.IsAny<Person>()));

        var controller = new PersonsController(mockRepo.Object);

        var newPerson = new Person
        {
            Name = "Lisa",
            Lastname = "Meier",
            Zipcode = "12345",
            City = "Musterstadt",
            Color = "grün"
        };

        // Act
        var result = controller.AddPerson(newPerson) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        var created = Assert.IsType<Person>(result!.Value);
        Assert.Equal("Lisa", created.Name);
    }
}
