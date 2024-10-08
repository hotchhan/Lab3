﻿/**
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
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace Lab3
{
    public partial class MainPage : ContentPage
    {
        private readonly IBusinessLogic _brains; // database 
        public ObservableCollection<Airport> Airports { get; set; } // getter and setter for the airports

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            _brains = new BusinessLogic(new Database()); // creates an instance of the business logic to use as the database
            Airports = new ObservableCollection<Airport>(_brains.GetAirports()); // gets the aiports from the database
            AirportsList.ItemsSource = Airports;
        }

        /// <summary>
        /// Ensures the input fields are the correct format then creates a new airport and then adds it to the collection when the add 
        /// airport button is clicked.
        /// </summary>
        /// <param name="sender">The on add button.</param>
        /// <param name="e">The id, city, and date visited for the airport.</param>
        private void OnAddAirportClicked(object sender, EventArgs e)
        {
            string id = IdEntry.Text;
            string city = CityEntry.Text;
            DateTime dateVisited;

            // Define acceptable date formats with MM/dd/yyyy format prioritized
            string[] formats = { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yyyy h:mm:ss tt" };

            // Try to parse using the exact formats defined above
            if (!DateTime.TryParseExact(DateVisitedEntry.Text.Trim(), formats, null, System.Globalization.DateTimeStyles.None, out dateVisited))
            {
                DisplayAlert("Error", "Invalid date format. Please use MM/DD/YYYY.", "OK");
                return;
            }

            // Format the date with time to MM/dd/yyyy h:mm:ss tt
            string formattedDate = dateVisited.ToString("MM/dd/yyyy h:mm:ss tt");

            int rating;
            if (!int.TryParse(RatingEntry.Text, out rating) || rating < 1 || rating > 5)
            {
                DisplayAlert("Error", "Rating must be between 1 and 5.", "OK");
                return;
            }

            // Create a new Airport object using the formatted date
            var newAirport = new Airport(id, city, DateTime.ParseExact(formattedDate, "MM/dd/yyyy h:mm:ss tt", null), rating);

            string result = _brains.AddAirport(id, city, newAirport.DateVisited, rating); // adds the airport to the database
            if (result.Contains("Error"))
            {
                DisplayAlert("Error", result, "OK"); // if the result has an error, display it as an alert
            }
            else
            {
                Airports.Add(newAirport); // if there is no error, add the airport and its information to the database
                ClearEntries(); // clears the text for the inputs
            }
        }

        /// <summary>
        /// Updates the selected airport's details with the values from the input fields when the edit airport button 
        /// is clicked.
        /// </summary>
        /// <param name="sender">The edit airport button.</param>
        /// <param name="e">The id, city and date visited inputs.</param>
        private void OnEditAirportClicked(object sender, EventArgs e)
        {
            // ensures an airport is selected
            var selectedAirport = (Airport)AirportsList.SelectedItem;
            if (selectedAirport == null)
            {
                DisplayAlert("Error", "Select an airport to edit.", "OK");
                return;
            }

            // chekc if any of the input fields are empty or null
            if (string.IsNullOrWhiteSpace(CityEntry.Text) || 
                string.IsNullOrWhiteSpace(DateVisitedEntry.Text) || 
                string.IsNullOrWhiteSpace(RatingEntry.Text))
            {
                DisplayAlert("Error", "All fields must be filled in before editing.", "OK");
                return;
            }

            // ensures the date format is correct for the date visited input
            if (!DateTime.TryParse(DateVisitedEntry.Text, out DateTime dateVisited))
            {
                DisplayAlert("Error", "Invalid date format. Please use MM/DD/YYYY.", "OK");
                return;
            }

            // ensures the rating field's value is between 1 and 5
            if (!int.TryParse(RatingEntry.Text, out int rating) || rating < 1 || rating > 5)
            {
                DisplayAlert("Error", "Rating must be a number between 1 and 5.", "OK");
                return;
            }

            string id = selectedAirport.Id; // ID is the same, does not change
            string city = CityEntry.Text; // city is taken from the input field

            string result = _brains.EditAirport(id, city, dateVisited, rating); // uses the business logic instance to update the airport information

            // updates the selected airport 
            selectedAirport.City = city;
            selectedAirport.DateVisited = dateVisited;
            selectedAirport.Rating = rating;

            AirportsList.ItemsSource = null; // refresh the collection view of airports
            AirportsList.ItemsSource = Airports;

            ClearEntries(); // clear all input fields
        }

        /// <summary>
        /// Removes the selected airport from the collection after confirming when the delete airport button (trash can) is clicked.
        /// </summary>
        /// <param name="sender">The trash can button.</param>
        /// <param name="e">The airport information.</param>
        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button) // checks that the object is the image button
            {
                if (button.BindingContext is Airport airport) // checks that the binding context is an Airport
                {
                    string result = _brains.DeleteAirport(airport); // calls the delete airport method and checks for errors
                    if (result.Contains("Error"))
                    {
                        DisplayAlert("Error", result, "OK"); // show error if there was an error as an alert
                    }
                    else
                    {
                        Airports?.Remove(airport); // if airport is not null remove it from the list
                    }
                }
                else
                {
                    DisplayAlert("Error", "Invalid airport data.", "OK"); // display an alert if the data is not an airport
                }
            }
            else
            {
                DisplayAlert("Error", "Invalid button data.", "OK"); // display an alert if the button is not the trash can (for deleting)
            }
        }

        /// <summary>
        /// Returns a status message based on the number of airports visited.
        /// </summary>
        public void CalculateStatistics()
        {
            var airports = _brains.GetAirports(); // Gets the list of all airports using the business logic layer
            var totalVisited = airports.Count; // Counts the total number of visited airports

            string status;
            int remainingVisits;

            // Determines the user's status based on the number of airports visited
            if (totalVisited >= 100)
            {
                DisplayAlert("Status", "You are the highest status!", "OK"); // Max status if 100 or more airports visited
                return;
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

            // Constructs the message to display
            string message;
            if (totalVisited == 1)
            {
                message = $"{totalVisited} airport visited; {remainingVisits} airports remaining until achieving {status}.";
            }
            else
            {
                message = $"{totalVisited} airports visited; {remainingVisits} airports remaining until achieving {status}.";
            }

            // Display the alert with the user's status and remaining visits
            DisplayAlert("Airport Visit Status", message, "OK");
        }


        /// <summary>
        /// Clears the input fields for ID, city, date visited, and rating.
        /// </summary>
        private void ClearEntries()
        {
            IdEntry.Text = string.Empty;
            CityEntry.Text = string.Empty;
            DateVisitedEntry.Text = string.Empty;
            RatingEntry.Text = string.Empty;
        }
    }
}