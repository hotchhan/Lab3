/**
Name: Hannah Hotchkiss, Carissa Engebose
Date: 10/1/24
Description: Lab 2, but now with a remote database
Bugs: The database cannot be connected to, so no airports are added to the database.
Because of this no airports can be deleted but you can edit them they just aren't
in the database. We know the credentials are correct, so we are unsure why
the authentication continues to fail. We also tried supabase and that didn't
work either.
**/

using System;

namespace Lab3
{
    /// <summary>
    /// Represents an airport with basic details such as ID, city, date visited, and rating.
    /// </summary>
    public class Airport
    {
        /// <summary>
        /// Gets or sets the unique identifier for the airport.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the city where the airport is located.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the date the airport was visited.
        /// </summary>
        public DateTime DateVisited { get; set; }

        /// <summary>
        /// Gets or sets the rating of the airport (1-5).
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Airport"/> class with specified details.
        /// </summary>
        /// <param name="id">The unique identifier of the airport.</param>
        /// <param name="city">The city where the airport is located.</param>
        /// <param name="dateVisited">The date the airport was visited.</param>
        /// <param name="rating">The rating of the airport.</param>
        public Airport(string id, string city, DateTime dateVisited, int rating)
        {
            Id = id;
            City = city;
            DateVisited = dateVisited;
            Rating = rating;
        }

        /// <summary>
        /// Gets the formatted date of the visit in "MM/dd/yyyy h:mm:ss tt" format.
        /// </summary>
        public string FormattedDate => DateVisited.ToString("MM/dd/yyyy h:mm:ss tt");

        /// <summary>
        /// Returns a string representation of the airport, including ID, city, formatted date, and rating.
        /// </summary>
        /// <returns>A string representation of the airport.</returns>
        public override string ToString()
        {
            return $"{Id} {City} {FormattedDate} {Rating}";
        }
    }
}