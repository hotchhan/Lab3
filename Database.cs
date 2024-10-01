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
using System.Data;
using System.IO;
using System.Linq;
using Npgsql;

namespace Lab3
{
    /// <summary>
    /// Provides methods to interact with the airport data stored in a text file.
    /// </summary>
    public class Database : IDatabase
    {
        // Builds a ConnectionString, which is used to connect to the database
        static String GetConnectionString()
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder();
            connStringBuilder.Host = "hotchkiss-1962.jxf.gcp-us-central1.cockroachlabs.cloud";
            connStringBuilder.Port = 26257;
            connStringBuilder.SslMode = SslMode.Require;
            connStringBuilder.Username = "username";  
            connStringBuilder.Password = "<password>";  
            connStringBuilder.Database = "defaultdb";  
            connStringBuilder.ApplicationName = "whatever"; 
            connStringBuilder.IncludeErrorDetail = true;
            return connStringBuilder.ConnectionString;
        }

        private static Random rng = new();
        private String connString = GetConnectionString();

        ObservableCollection<Airport> airports = new();

        /// <summary>
        /// Selects all airports from the database.
        /// </summary>
        /// <returns>An observable collection of airports.</returns>
        public ObservableCollection<Airport> SelectAllAirports()
        {
            airports.Clear();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM airports", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                String airportId = reader.GetString(0);
                String city = reader.GetString(1);
                DateTime dateVisited = reader.GetDateTime(2);
                Int32 rating = reader.GetInt32(3);

                Airport airportToAdd = new(airportId, city, dateVisited, rating);
                airports.Add(airportToAdd);
                Console.WriteLine($"Airport added: {airportToAdd}");
            }

            return airports;
        }

        /// <summary>
        /// Inserts a new airport into the database.
        /// </summary>
        /// <param name="airport">The airport to insert.</param>
        /// <returns>A message indicating the result of the insertion.</returns>
        public string InsertAirport(Airport airport)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
                Console.WriteLine("Database connection opened.");

                using var transaction = conn.BeginTransaction();
                using var cmd = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = "INSERT INTO airports (id, city, date_visited, rating) VALUES (@id, @city, @dateVisited, @rating)"
                };

                // Set parameter values
                cmd.Parameters.AddWithValue("id", airport.Id);
                cmd.Parameters.AddWithValue("city", airport.City);
                cmd.Parameters.AddWithValue("dateVisited", airport.DateVisited);
                cmd.Parameters.AddWithValue("rating", airport.Rating);

                // Execute the command and check the number of rows affected
                int numRowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"Rows affected: {numRowsAffected}");

                // Commit the transaction if successful
                transaction.Commit();
                Console.WriteLine("Transaction committed.");

                if (numRowsAffected > 0)
                {
                    airports.Add(airport); // Add the airport to the local collection
                    return "Airport added successfully.";
                }
                else
                {
                    return "No rows were affected. The airport was not added.";
                }
            }
            catch (Npgsql.PostgresException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}"); // Log error message
                Console.WriteLine($"SQL State: {ex.SqlState}");    // Log SQL state
                Console.WriteLine($"Error Code: {ex.ErrorCode}");  // Log error code
                Console.WriteLine($"Detail: {ex.Detail}");         // Log detailed error
                return $"Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return $"General error: {ex.Message}";
            }
        }

        /// <summary>
        /// Deletes an airport from the database by its ID.
        /// </summary>
        /// <param name="airportToDelete">The airport object to delete.</param>
        /// <returns>A message indicating the result of the deletion.</returns>
        public string DeleteAirport(Airport airportToDelete)
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "DELETE FROM airports WHERE id = @id";
            cmd.Parameters.AddWithValue("id", airportToDelete.Id);

            int numDeleted = cmd.ExecuteNonQuery();
            if (numDeleted > 0)
            {
                Console.WriteLine("Airport deleted successfully.");
                SelectAllAirports(); // Refresh data
                return "Airport deleted successfully.";
            }
            else
            {
                Console.WriteLine("No airport found with the given ID.");
                return "Error while deleting Airport: No airport found with the given ID.";
            }
        }


        /// <summary>
        /// Updates an existing airport's details in the database.
        /// </summary>
        /// <param name="airportToUpdate">The airport object with updated information.</param>
        /// <returns>A message indicating whether the airport was successfully updated or not.</returns>
        public string UpdateAirport(Airport airportToUpdate)
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "UPDATE airports SET city = @city, date_visited = @date_visited, rating = @rating WHERE id = @id";
            cmd.Parameters.AddWithValue("id", airportToUpdate.Id);
            cmd.Parameters.AddWithValue("city", airportToUpdate.City);
            cmd.Parameters.AddWithValue("date_visited", airportToUpdate.DateVisited);
            cmd.Parameters.AddWithValue("rating", airportToUpdate.Rating);

            int numAffected = cmd.ExecuteNonQuery();
            if (numAffected > 0)
            {
                Console.WriteLine("Airport updated successfully.");
                SelectAllAirports(); // Refresh data
                return "Airport updated successfully.";
            }
            else
            {
                Console.WriteLine("No airport found with the given ID.");
                return "Error while updating Airport: No airport found with the given ID.";
            }
        }
    }
}

