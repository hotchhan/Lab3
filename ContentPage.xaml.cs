/**
Name: Hannah Hotchkiss, Carissa Engebose
Date: 10/1/24
Description: Lab 2, but now with a remote database and data persistence
Bugs: None known.
**/

using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace Lab3
{
    public partial class MainPage : ContentPage
    {
        private readonly IBusinessLogic _brains; // Database connection via business logic
        public ObservableCollection<Airport> Airports { get; set; } // Local collection of airports

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            _brains = new BusinessLogic(new Database()); // Initialize business logic with database connection
            
            // Load data from the database when the app starts
            LoadAirportsFromDatabase();
        }

        /// <summary>
        /// Fetches all airports from the database and populates the local collection.
        /// </summary>
        private void LoadAirportsFromDatabase()
        {
            // Fetch airports from the database
            var airportsFromDb = _brains.GetAirports();
            
            // Initialize the Airports collection with the fetched data
            Airports = new ObservableCollection<Airport>(airportsFromDb);
            
            // Bind the collection to the UI
            AirportsList.ItemsSource = Airports;
        }

        /// <summary>
        /// Ensures the input fields are the correct format, then creates a new airport and adds it to the collection when the add 
        /// airport button is clicked.
        /// </summary>
        /// <param name="sender">The on add button.</param>
        /// <param name="e">The id, city, and date visited for the airport.</param>
        private void OnAddAirportClicked(object sender, EventArgs e)
        {
            // Validate ID field
            if (string.IsNullOrWhiteSpace(IdEntry.Text))
            {
                DisplayAlert("Error", "ID cannot be blank.", "OK");
                return;
            }

            // Check if an airport with the same ID already exists in the collection
            if (Airports.Any(a => a.Id == IdEntry.Text))
            {
                DisplayAlert("Error", "An airport with the same ID already exists.", "OK");
                return;
            }

            // Validate city field
            if (string.IsNullOrWhiteSpace(CityEntry.Text))
            {
                DisplayAlert("Error", "City cannot be blank.", "OK");
                return;
            }

            // Validate date field
            string[] formats = { "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss tt", "M/d/yyyy", "M/d/yyyy h:mm:ss tt" };
            if (string.IsNullOrWhiteSpace(DateVisitedEntry.Text) || !DateTime.TryParseExact(DateVisitedEntry.Text.Trim(), formats, null, System.Globalization.DateTimeStyles.None, out DateTime dateVisited))
            {
                DisplayAlert("Error", "Invalid date format or blank date. Please use MM/DD/YYYY.", "OK");
                return;
            }

            // Validate rating field
            if (string.IsNullOrWhiteSpace(RatingEntry.Text) || !int.TryParse(RatingEntry.Text, out int rating) || rating < 1 || rating > 5)
            {
                DisplayAlert("Error", "Rating must be between 1 and 5.", "OK");
                return;
            }

            // Format the date with time
            string formattedDate = dateVisited.ToString("MM/dd/yyyy h:mm:ss tt");

            // Create a new Airport object
            var newAirport = new Airport(IdEntry.Text, CityEntry.Text, DateTime.ParseExact(formattedDate, "MM/dd/yyyy h:mm:ss tt", null), rating);

            // Attempt to add the airport to the database
            string result = _brains.AddAirport(IdEntry.Text, CityEntry.Text, newAirport.DateVisited, rating); // adds the airport to the database
            if (result.Contains("Error"))
            {
                DisplayAlert("Error", result, "OK"); // if the result has an error, display it as an alert
            }
            else
            {
                // Add the new airport to the local collection and UI
                Airports.Add(newAirport);
                RefreshUI(); // Refresh the list and UI
                ClearEntries(); // Clear input fields
            }
        }

        /// <summary>
        /// Updates the selected airport's details with the values from the input fields when the edit airport button 
        /// is clicked.
        /// </summary>
        /// <param name="sender">The edit airport button.</param>
        /// <param name="e">The id, city, and date visited inputs.</param>
        private void OnEditAirportClicked(object sender, EventArgs e)
        {
            // Ensure an airport is selected
            var selectedAirport = (Airport)AirportsList.SelectedItem;
            if (selectedAirport == null)
            {
                DisplayAlert("Error", "Select an airport to edit.", "OK");
                return;
            }

            // Validate city field
            if (string.IsNullOrWhiteSpace(CityEntry.Text))
            {
                DisplayAlert("Error", "City cannot be blank.", "OK");
                return;
            }

            // Validate date field
            if (string.IsNullOrWhiteSpace(DateVisitedEntry.Text) || !DateTime.TryParse(DateVisitedEntry.Text, out DateTime dateVisited))
            {
                DisplayAlert("Error", "Invalid date format or blank date. Please use MM/DD/YYYY.", "OK");
                return;
            }

            // Validate rating field
            if (string.IsNullOrWhiteSpace(RatingEntry.Text) || !int.TryParse(RatingEntry.Text, out int rating) || rating < 1 || rating > 5)
            {
                DisplayAlert("Error", "Rating must be a number between 1 and 5.", "OK");
                return;
            }

            // Update the selected airport 
            string id = selectedAirport.Id; // ID is the same, does not change
            string city = CityEntry.Text; // city is taken from the input field

            string result = _brains.EditAirport(id, city, dateVisited, rating); // uses the business logic instance to update the airport information

            if (!result.Contains("Error"))
            {
                // Update the airport in the collection
                selectedAirport.City = city;
                selectedAirport.DateVisited = dateVisited;
                selectedAirport.Rating = rating;

                RefreshUI(); // Refresh the list and UI
                ClearEntries(); // Clear input fields
            }
            else
            {
                DisplayAlert("Error", result, "OK");
            }
        }

        /// <summary>
        /// Removes the selected airport from the collection after confirming when the delete airport button is clicked.
        /// </summary>
        /// <param name="sender">The trash can button.</param>
        /// <param name="e">The airport information.</param>
        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button) // checks that the object is the image button
            {
                if (button.BindingContext is Airport airport) // checks that the binding context is an Airport
                {
                    string result = _brains.DeleteAirport(airport.Id); // calls the delete airport method and checks for errors
                    if (result.Contains("Error"))
                    {
                        DisplayAlert("Error", result, "OK"); // show error if there was an error as an alert
                    }
                    else
                    {
                        Airports?.Remove(airport); // if airport is not null remove it from the list
                        RefreshUI(); // Refresh the list and UI
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
        /// Refreshes the UI by rebinding the updated Airports collection.
        /// </summary>
        private void RefreshUI()
        {
            AirportsList.ItemsSource = null; // Unbind the current data source
            AirportsList.ItemsSource = Airports; // Rebind the updated collection
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
        
        /// <summary>
        /// Handles the click event for the Calculate Statistics button.
        /// </summary>
        private void OnCalculateStatisticsClicked(object sender, EventArgs e)
        {
            // No need to re-fetch from the database here. Use the existing Airports collection.
            var totalVisited = Airports.Count; // Use the updated local collection

            string status;
            int remainingVisits;

            // Determine status based on the number of airports visited
            if (totalVisited >= 100)
            {
                status = "highest";
                remainingVisits = 0; // No more visits needed
            }
            else if (totalVisited >= 60)
            {
                status = "Gold";
                remainingVisits = 100 - totalVisited; // Remaining visits for highest status
            }
            else if (totalVisited >= 42)
            {
                status = "Silver";
                remainingVisits = 60 - totalVisited; // Remaining visits for Gold status
            }
            else
            {
                status = "Bronze";
                remainingVisits = 42 - totalVisited; // Remaining visits for Silver status
            }

            // Build the result message
            string resultMessage = totalVisited == 1
                ? $"{totalVisited} airport visited; {remainingVisits} remaining until {status} status."
                : $"{totalVisited} airports visited; {remainingVisits} remaining until {status} status.";

            // Display the result in an alert
            DisplayAlert("Statistics", resultMessage, "OK");
        }
    }
}
