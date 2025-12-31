using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using PersonsApi.Core;

namespace PersonsApi.Controllers
{
    /// <summary>
    /// REST controller providing endpoints for managing persons.
    /// </summary>
    [ApiController]
    [Route("persons")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonsController"/>.
        /// The repository is injected via dependency injection.
        /// </summary>
        /// <param name="repository">Person repository abstraction</param>
        public PersonsController(IPersonRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns all persons.
        /// </summary>
        /// <returns>List of all persons</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAll());
        }

        /// <summary>
        /// Returns a single person by its identifier.
        /// </summary>
        /// <param name="id">Person identifier (row number)</param>
        /// <returns>The matching person or 404 if not found</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var person = _repository.GetById(id);

            if (person is null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        /// <summary>
        /// Returns all persons matching the given color.
        /// </summary>
        /// <param name="color">Color name to filter by</param>
        /// <returns>List of persons with the given color</returns>
        [HttpGet("color/{color}")]
        public IActionResult GetByColor(string color)
        {
            var persons = _repository.GetByColor(color);
            return Ok(persons);
        }

        /// <summary>
        /// Adds a new person at runtime.
        /// The CSV file itself is not modified; the entry is stored in memory.
        /// </summary>
        /// <param name="person">Person to be added</param>
        /// <returns>The created person including its assigned ID</returns>
        [HttpPost]
        public IActionResult AddPerson([FromBody] Person person)
        {
            // Assign a new ID based on the current maximum ID
            var maxId = _repository.GetAll().Any()
                ? _repository.GetAll().Max(p => p.Id)
                : 0;

            person.Id = maxId + 1;

            _repository.Add(person);

            return CreatedAtAction(
                nameof(GetById),
                new { id = person.Id },
                person
            );
        }
    }
}
