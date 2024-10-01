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
    /// Defines the business logic related to managing the airports.
    /// </summary>
    public interface IBusinessLogic
    {
        /// <summary>
        /// Retrieves a list of all airports.
        /// </summary>
        /// <returns>A list of airports.</returns>
        ObservableCollection<Airport> ListAirports();

        /// <summary>
        /// Adds a new airport to the database.
        /// </summary>
        /// <param name="id">The ID of the new airport.</param>
        /// <param name="city">The city where the airport is located.</param>
        /// <param name="dateVisited">The date the airport was visited.</param>
        /// <param name="rating">The rating of the airport (1 to 5).</param>
        /// <returns>A message saying if the airport was successfully added or not.</returns>
        string AddAirport(string id, string city, DateTime dateVisited, int rating);

        /// <summary>
        /// Deletes an airport by its ID.
        /// </summary>
        /// <param name="airport">The ID of the airport to delete.</param>
        /// <returns>A message saying if the airport was successfully deleted or not.</returns>
        string DeleteAirport(string airport);

        /// <summary>
        /// Edits an existing airport's details.
        /// </summary>
        /// <param name="id">The ID of the airport to edit.</param>
        /// <param name="city">The updated city of the airport.</param>
        /// <param name="dateVisited">The updated date the airport was visited.</param>
        /// <param name="rating">The updated rating of the airport (1 to 5).</param>
        /// <returns>A message saying if the airport was successfully updated or not.</returns>
        string EditAirport(string id, string city, DateTime dateVisited, int rating);

        /// <summary>
        /// Calculates and returns statistics related to the airports.
        /// </summary>
        /// <returns>A message containing the calculated statistics.</returns>
        string CalculateStatistics();

        /// <summary>
        /// Gets the list of airports from the data source.
        /// </summary>
        /// <returns>A list of airports.</returns>
        ObservableCollection<Airport> GetAirports();
    }
}
