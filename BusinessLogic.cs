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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lab3
{
    /// <summary>
    /// Provides business logic operations for managing airports.
    /// </summary>
    public class BusinessLogic : IBusinessLogic
    {
        private readonly IDatabase _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessLogic"/> class.
        /// </summary>
        /// <param name="database">The database to interact with.</param>
        public BusinessLogic(IDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Retrieves the list of all airports.
        /// </summary>
        /// <returns>A list of airports.</returns>
        public ObservableCollection<Airport> ListAirports()
        {
            return _database.SelectAllAirports();
        }

        /// <summary>
        /// Adds a new airport to the system.
        /// </summary>
        /// <param name="id">The ID of the airport.</param>
        /// <param name="city">The city where the airport is located.</param>
        /// <param name="dateVisited">The date the airport was visited.</param>
        /// <param name="rating">The rating of the airport (1 to 5).</param>
        /// <returns>A message saying if the airport was successfully added or not.</returns>
        public string AddAirport(string id, string city, DateTime dateVisited, int rating)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(city)) // check if the fields are null
            {
                return "Error: All fields must be filled in.";
            }
            if (id.Length < 3 || id.Length > 4) // check if the ID is 3 or 4 characters
            {
                return "Error: Airport ID must be 3-4 characters long.";
            }
            if (rating < 1 || rating > 5) // check if rating is between 1 and 5
            {
                return "Error: Rating must be between 1 and 5.";
            }
            var airport = new Airport(id, city, dateVisited, rating); // create a new airport with the inputed values

            return _database.InsertAirport(airport); // return if the airport was inserted into the database
        }

        /// <summary>
        /// Deletes an airport by its ID.
        /// </summary>
        /// <param name="airport">The ID of the airport to delete.</param>
        /// <returns>A message saying if the airport was successfully deleted from the database or not.</returns>
        public string DeleteAirport(Airport airport)
        {
            return _database.DeleteAirport(airport);
        }

        /// <summary>
        /// Edits an existing airport's details.
        /// </summary>
        /// <param name="id">The ID of the airport to edit.</param>
        /// <param name="city">The updated city of the airport.</param>
        /// <param name="dateVisited">The updated date the airport was visited.</param>
        /// <param name="rating">The updated rating of the airport (1 to 5).</param>
        /// <returns>A message saying if the airport was updated.</returns>
        public string EditAirport(string id, string city, DateTime dateVisited, int rating)
        {
            if (rating < 1 || rating > 5) // check if rating in valid
            {
                return "Error: Rating must be between 1 and 5.";
            }

            var airport = new Airport(id, city, dateVisited, rating);
            return _database.UpdateAirport(airport); // returns if the airport was updated with the new values
        }

        /// <summary>
        /// Calculates and returns statistics about the airports visited and the average rating.
        /// </summary>
        /// <returns>A string with the number of airports visited and the average rating.</returns>
        public string CalculateStatistics()
        {
            var airports = _database.SelectAllAirports();
            var totalVisited = airports.Count;

            if (totalVisited == 0) // checks if visited no airports
            {
                return "No airports visited yet.";
            }

            double averageRating = 0;
            foreach (var airport in airports)
            {
                averageRating += airport.Rating;
            }
            averageRating /= totalVisited; // calculates average rating of airports

            return $"{totalVisited} airports visited. Average rating: {averageRating:F1}"; // returns the statistics about the airport
        }

        /// <summary>
        /// Gets a list of all airports from the database.
        /// </summary>
        /// <returns>A list of airports.</returns>
        public ObservableCollection<Airport> GetAirports()
        {
            return _database.SelectAllAirports();
        }
    }
}
