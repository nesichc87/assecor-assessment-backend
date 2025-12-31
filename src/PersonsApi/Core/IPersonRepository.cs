using System.Collections.Generic;

namespace PersonsApi.Core
{
    /// <summary>
    /// Defines a contract for accessing and managing person data.
    /// </summary>
    public interface IPersonRepository
    {
        /// <summary>
        /// Returns all available persons.
        /// </summary>
        /// <returns>
        /// An enumerable collection of all persons from the data source
        /// and any in-memory additions.
        /// </returns>
        IEnumerable<Person> GetAll();

        /// <summary>
        /// Returns a single person by its unique identifier.
        /// </summary>
        /// <param name="id">The person identifier.</param>
        /// <returns>
        /// The matching <see cref="Person"/> if found; otherwise <c>null</c>.
        /// </returns>
        Person? GetById(int id);

        /// <summary>
        /// Returns all persons that match the specified color.
        /// </summary>
        /// <param name="color">The color to filter by.</param>
        /// <returns>
        /// A collection of persons associated with the given color.
        /// </returns>
        IEnumerable<Person> GetByColor(string color);

        /// <summary>
        /// Adds a new person at runtime.
        /// The underlying CSV file remains unchanged.
        /// </summary>
        /// <param name="person">The person to add.</param>
        void Add(Person person);
    }
}
