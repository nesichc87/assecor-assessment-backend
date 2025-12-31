using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PersonsApi.Core;

namespace PersonsApi.Infrastructure
{
    /// <summary>
    /// Repository implementation that reads persons from a CSV file
    /// and allows adding additional persons in memory.
    /// </summary>
    public class CsvPersonRepository : IPersonRepository
    {
        private readonly string _csvFilePath;
        private readonly List<Person> _inMemoryPersons = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvPersonRepository"/> class.
        /// </summary>
        /// <param name="csvFilePath">Path to the CSV file containing person records.</param>
        public CsvPersonRepository(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }

        /// <summary>
        /// Returns all persons from the CSV file and all persons added in memory.
        /// </summary>
        /// <returns>A collection of all persons.</returns>
        public IEnumerable<Person> GetAll()
        {
            if (!File.Exists(_csvFilePath))
            {
                return _inMemoryPersons;
            }

            var csvPersons = File.ReadAllLines(_csvFilePath)
                .Select((line, index) => ParseLine(line, index + 1))
                .Where(p => p != null)!
                .Cast<Person>();

            return csvPersons.Concat(_inMemoryPersons).ToList();
        }

        /// <summary>
        /// Returns a person by its identifier.
        /// </summary>
        /// <param name="id">The person identifier (CSV line number).</param>
        /// <returns>The matching person or <c>null</c> if not found.</returns>
        public Person? GetById(int id)
        {
            return GetAll().FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Returns all persons matching the given color.
        /// </summary>
        /// <param name="color">The color name to filter by.</param>
        /// <returns>A collection of persons with the specified color.</returns>
        public IEnumerable<Person> GetByColor(string color)
        {
            return GetAll()
                .Where(p => string.Equals(p.Color, color, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Adds a new person to the in-memory data store.
        /// The CSV file itself is not modified.
        /// </summary>
        /// <param name="person">The person to add.</param>
        public void Add(Person person)
        {
            _inMemoryPersons.Add(person);
        }

        /// <summary>
        /// Parses a single CSV line into a <see cref="Person"/> instance.
        /// </summary>
        /// <param name="line">The CSV line to parse.</param>
        /// <param name="lineNumber">The line number used as the person identifier.</param>
        /// <returns>
        /// A <see cref="Person"/> instance if parsing was successful;
        /// otherwise <c>null</c>.
        /// </returns>
        private static Person? ParseLine(string line, int lineNumber)
        {
            var parts = line.Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length < 4)
                return null;

            if (!int.TryParse(parts[^1], out int colorId))
                return null;

            var zipCity = parts[2].Split(' ', 2);

            return new Person
            {
                Id = lineNumber,
                Lastname = parts[0],
                Name = parts[1],
                Zipcode = zipCity[0],
                City = zipCity.Length > 1 ? zipCity[1] : string.Empty,
                Color = ColorMapper.FromId(colorId)
            };
        }
    }
}