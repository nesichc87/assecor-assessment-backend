namespace PersonsApi.Core
{
    /// <summary>
    /// Maps color identifiers from the CSV source to human-readable color names.
    /// </summary>
    public static class ColorMapper
    {
        private static readonly Dictionary<int, string> Map = new()
        {
            { 1, "blau" },
            { 2, "grün" },
            { 3, "violett" },
            { 4, "rot" },
            { 5, "gelb" },
            { 6, "türkis" },
            { 7, "weiß" }
        };

        /// <summary>
        /// Returns the color name for a given color identifier.
        /// </summary>
        /// <param name="id">The numeric color identifier.</param>
        /// <returns>The mapped color name or "unknown" if the id is unknown.</returns>
        public static string FromId(int id)
        {
            return Map.TryGetValue(id, out var color)
                ? color
                : "unknown";
        }
    }
}