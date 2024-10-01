/**
Name: Hannah Hotchkiss, Carissa Engebose
Date: 10/1/24
Description: Lab 2, but now with a remote database and data persistence
Bugs: None known.
**/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Lab3
{
    /// <summary>
    /// Provides methods to interact with the airport data stored in a PostgreSQL database.
    /// </summary>
    public class Database : IDatabase
    {
        // Connection string to the PostgreSQL database.
        private readonly string connString;

        // ObservableCollection to store the list of airports.
        private ObservableCollection<Airport> airports = new ObservableCollection<Airport>();

        // Property to expose the list of airports.
        public ObservableCollection<Airport> Airports => airports;

        // Constructor that sets up the connection string and fetches all airports.
        public Database()
        {
            connString = GetConnectionString();
            Console.WriteLine($"Connected to database: {connString}");
            SelectAllAirports(); // Fetch all airports initially
        }


        /// <summary>
        /// Gets all airports from the database.
        /// </summary>
        /// <returns>A list of all airports.</returns>
        public ObservableCollection<Airport> SelectAllAirports()
        {
            airports.Clear(); // Clear the local collection

            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM airports", conn);
                using var reader = cmd.ExecuteReader();

                // Loop through each row in the result and add it to the airports collection.
                while (reader.Read())
                {
                    var id = reader.GetString(0);
                    var city = reader.GetString(1);
                    var dateVisited = reader.GetDateTime(2);
                    var rating = reader.GetInt32(3);
                    var airport = new Airport(id, city, dateVisited, rating);
                    airports.Add(airport); // Add to the collection
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                // Handle any database-specific errors.
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return airports;
        }

        /// <summary>
        /// Gets a specific airport by its ID.
        /// </summary>
        /// <param name="id">The ID of the airport to retrieve.</param>
        /// <returns>The airport if it exists; otherwise, null.</returns>
        public ObservableCollection<Airport> SelectAirport(string id)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM airports WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("id", id); // Set the parameter value.
                using var reader = cmd.ExecuteReader();

                // If an airport is found, add it to the local collection.
                if (reader.Read())
                {
                    var city = reader.GetString(1);
                    var dateVisited = reader.GetDateTime(2);
                    var rating = reader.GetInt32(3);
                    var airport = new Airport(id, city, dateVisited, rating);
                    airports.Add(airport);
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                // Handle any database-specific errors.
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return airports; // Return the collection of airports.
        }

        /// <summary>
        /// Inserts a new airport into the database with simplified error handling.
        /// </summary>
        /// <param name="airport">The airport to insert.</param>
        /// <returns>A message indicating if the airport was successfully inserted or not.</returns>
        public string InsertAirport(Airport airport)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                // Start a transaction for the insertion.
                using var transaction = conn.BeginTransaction();
                using var cmd = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = "INSERT INTO airports (id, city, date_visited, rating) VALUES (@id, @city, @dateVisited, @rating)"
                };

                // Set parameter values for the insert query.
                cmd.Parameters.AddWithValue("id", airport.Id);
                cmd.Parameters.AddWithValue("city", airport.City);
                cmd.Parameters.AddWithValue("dateVisited", airport.DateVisited);
                cmd.Parameters.AddWithValue("rating", airport.Rating);

                // Execute the command and check the number of rows affected.
                int numRowsAffected = cmd.ExecuteNonQuery();

                // Commit the transaction if the insertion is successful.
                transaction.Commit();

                if (numRowsAffected > 0)
                {
                    airports.Add(airport); // Add the airport to the local collection.
                    return "Airport added successfully.";
                }
                else
                {
                    return "No rows were affected. The airport was not added.";
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                // Handle any database-specific errors.
                Console.WriteLine($"Database error: {ex.Message}");
                return "An error occurred while adding the airport.";
            }
            catch (Exception ex)
            {
                // Handle general errors.
                Console.WriteLine($"General error: {ex.Message}");
                return "Error adding airport.";
            }
        }

        /// <summary>
        /// Deletes an airport from the database by its ID from an Airport object.
        /// </summary>
        /// <param name="airportId">The Airport object containing the ID of the airport to delete.</param>
        /// <returns>A message indicating whether the airport was deleted from the database.</returns>
        public string DeleteAirport(string airportId)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                using var cmd = new NpgsqlCommand("DELETE FROM airports WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("id", airportId); // Use the airport ID directly.

                int numRowsAffected = cmd.ExecuteNonQuery();

                // If the deletion is successful, remove the airport from the local collection.
                if (numRowsAffected > 0)
                {
                    var airportToDelete = airports.FirstOrDefault(a => a.Id == airportId);
                    if (airportToDelete != null)
                    {
                        airports.Remove(airportToDelete); // Update the local collection.
                    }
                    return "Airport deleted successfully.";
                }
                else
                {
                    return "Airport not found in the database.";
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                // Handle any database-specific errors.
                Console.WriteLine($"Database error: {ex.Message}");
                return $"Error while deleting Airport from the database: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle general errors.
                Console.WriteLine($"General error: {ex.Message}");
                return $"An unexpected error occurred while deleting the Airport: {ex.Message}";
            }
        }

        /// <summary>
        /// Updates an existing airport's details in the database.
        /// </summary>
        /// <param name="airport">The airport object with updated information.</param>
        /// <returns>A message indicating whether the airport was successfully updated or not.</returns>
        public string UpdateAirport(Airport airport)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                // Set up the update command.
                var cmd = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = "UPDATE airports SET city = @city, date_visited = @dateVisited, rating = @rating WHERE id = @id"
                };
                cmd.Parameters.AddWithValue("id", airport.Id);
                cmd.Parameters.AddWithValue("city", airport.City);
                cmd.Parameters.AddWithValue("dateVisited", airport.DateVisited);
                cmd.Parameters.AddWithValue("rating", airport.Rating);

                // Execute the update command and check if rows were affected.
                int numRowsAffected = cmd.ExecuteNonQuery();
                if (numRowsAffected > 0)
                {
                    var index = airports.IndexOf(airports.First(a => a.Id == airport.Id));
                    if (index >= 0)
                    {
                        airports[index] = airport; // Update the local collection.
                    }
                    return "Airport updated successfully.";
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                // Handle any database-specific errors.
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return "Error while updating Airport.";
        }

        /// <summary>
        /// Builds a ConnectionString, which is used to connect to the database.
        /// </summary>
        /// <returns>The connection string to the PostgreSQL database.</returns>
        static string GetConnectionString()
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder();

            connStringBuilder.Host = "posh-bulldog-13394.5xj.gcp-us-central1.cockroachlabs.cloud";
            connStringBuilder.Port = 26257;  // Default CockroachDB port
            connStringBuilder.SslMode = SslMode.Require;  // SSL mode to ensure security
            connStringBuilder.Username = "carissae";  // Your CockroachDB username
            connStringBuilder.Password = "newpassword123!";  // Your CockroachDB password
            connStringBuilder.Database = "defaultdb";  // Database name
            connStringBuilder.ApplicationName = "Lab3";  // You can set this to your app name

            return connStringBuilder.ConnectionString;
        }

    }
}
