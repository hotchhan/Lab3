/**
Name: Hannah Hotchkiss, Carissa Engebose
Date: 10/1/24
Description: Lab 2, but now with a remote database and data persistence
Bugs: None known.
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
        /// <param name="id">The ID of the airport to delete.</param>
        /// <returns>A message saying if the airport was successfully deleted from the database or not.</returns>
        public string DeleteAirport(string id)
        {
            return _database.DeleteAirport(id);
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
    /// Returns a status message based on the number of airports visited.
    /// </summary>
    /// <returns>A string describing the user's current status and how many more visits are required for the next tier.</returns>
    public string CalculateStatistics()
    {
        var airports = _database.SelectAllAirports(); // Gets the list of all airports
        var totalVisited = airports.Count; // Counts the total number of visited airports
        
        string status;
        int remainingVisits;

        // Determines the user's status based on the number of airports visited
        if (totalVisited >= 100) 
        {
            return $"You are the highest status!"; // Max status if 100 or more airports visited
        } 
        else if (totalVisited >= 42) // Silver status requires 42 or more airports
        {
            status = "Silver";
            remainingVisits = Math.Max(0, 60 - totalVisited); // Calculates how many more visits are needed for the next tier
        }
        else
        {
            status = "Bronze"; // Default status if fewer than 42 airports visited
            remainingVisits = Math.Max(0, 42 - totalVisited); // Calculates how many more visits are needed for Silver tier
        }

        // Returns a message with the total visited airports and how many more are needed for the next status tier
        if (totalVisited == 1) 
        {
            return $"\n{totalVisited} airport visited; {remainingVisits} airports remaining until achieving {status}.";
        } 
        else 
        {
            return $"\n{totalVisited} airports visited; {remainingVisits} airports remaining until achieving {status}.";
        }
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
