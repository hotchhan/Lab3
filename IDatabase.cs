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
    // <summary>
    /// Defines what can be performed on the Airport database.
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// Gets a list of all airports from the database.
        /// </summary>
        /// <returns>A list of all airports.</returns>
        ObservableCollection<Airport> SelectAllAirports();

        /// <summary>
        /// Gets a specific airport by its ID.
        /// </summary>
        /// <param name="id">The ID of the airport to find.</param>
        /// <returns>The airport object if exists; otherwise, null.</returns>
        ObservableCollection<Airport> SelectAirport(string id);

        /// <summary>
        /// Inserts a new airport into the database.
        /// </summary>
        /// <param name="airport">The airport to insert.</param>
        /// <returns>A message saying if the airport was inserted into the database or not.</returns>
        string InsertAirport(Airport airport);

        /// <summary>
        /// Deletes an airport from the database by its ID.
        /// </summary>
        /// <param name="airport">The ID of the airport to delete.</param>
        /// <returns>A message saying if the airport was deleted from the database or not.</returns>
        string DeleteAirport(string airport);

        /// <summary>
        /// Updates an existing airport in the database.
        /// </summary>
        /// <param name="airport">The airport with updated information.</param>
        /// <returns>A message saying if the airport was updated in the database or not.</returns>
        string UpdateAirport(Airport airport);
    }
}
