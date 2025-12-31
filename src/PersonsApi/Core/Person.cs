namespace PersonsApi.Core
{
    /// <summary>
    /// Represents a person with basic personal information.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Unique identifier of the person (derived from the CSV row number).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// First name of the person.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the person.
        /// </summary>
        public string Lastname { get; set; } = string.Empty;

        /// <summary>
        /// Postal code of the person's address.
        /// </summary>
        public string Zipcode { get; set; } = string.Empty;

        /// <summary>
        /// City of residence.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Color associated with the person (mapped from a color ID).
        /// </summary>
        public string Color { get; set; } = string.Empty;
    }
}

