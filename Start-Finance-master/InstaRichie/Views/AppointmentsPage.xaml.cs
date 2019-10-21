using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;


namespace StartFinance.Views
{
    /// <summary>
    /// Appointments Page that will display a form to input data and select a Date and Time of Appointment. You can submit the Appointment and it will display the confirmed details
    /// It will so allow you to update or delete the data on selected appointment item.
    /// </summary>
    public sealed partial class AppointmentsPage : Page
    {
        // Adding an SQLite connection
        SQLiteConnection conn; 
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public string selectedItem = "";

        public AppointmentsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            // Initializing a database connection
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Show results of table when page loads (data has been saved to database)
            Results();
        }

        private void Results()
        {
            // Find results in the database and add them to the view
            conn.CreateTable<Appointments>();
            var query1 = conn.Table<Appointments>();
            AppointmentsView.ItemsSource = query1.ToList(); //this view is added in the xaml code

        }

        public void ClearFields()
        {
            FirstNametxtBox.Text = "";
            LastNametxtBox.Text = "";
        }

        // Add button
        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check user has entered a name
                if (FirstNametxtBox.Text.ToString() == "" || LastNametxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Please enter First and Last Name", "Oh dear!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //Connect to table and add information
                    conn.CreateTable<Appointments>();
                    String AppointmentDateString;
                    AppointmentDateString = DOAdatePicker.Date.Day.ToString();
                    AppointmentDateString += "/" + DOAdatePicker.Date.Month.ToString();
                    AppointmentDateString += "/" + DOAdatePicker.Date.Year.ToString();
                    String AppointmentTimeString = TOAtimePicker.Time.ToString();
                    conn.Insert(new Appointments
                    {
                        FirstName = FirstNametxtBox.Text,
                        LastName = LastNametxtBox.Text,
                        DateOfAppointment = AppointmentDateString,
                        TimeOfAppointmant = AppointmentTimeString
                    });
                    ClearFields();
                    //Reload results to reflect new data
                    Results();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //Delete Button
        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (selectedItem == "")
                {

                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();

                    var query2 = conn.Query<Appointments>("DELETE FROM Appointments WHERE AppointmentID ='" + selectedItem + "'");
                    AppointmentsView.ItemsSource = query1.ToList();
                    selectedItem = "";
                    ClearFields();
                }
            }
            catch (NullReferenceException)
            {
                //Prevents a crash from pressing delete button while no item selected
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Have you selected an item?", "Oh dear..!");
                await dialog.ShowAsync();
            }
        }

        //Update Button
        private async void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {//Add code set fields to selected item values
                string newFirstName = FirstNametxtBox.Text;
                string newLastName = LastNametxtBox.Text;

                DateTime newDOA = DOAdatePicker.Date.DateTime;
                string DOAstring;
                DOAstring = DOAdatePicker.Date.Day.ToString() + "/" + DOAdatePicker.Date.Month.ToString() + "/" + DOAdatePicker.Date.Year.ToString();

                string newTOA = TOAtimePicker.Time.ToString();



                if (selectedItem == "")
                {
                    MessageDialog dialog = new MessageDialog("You need to select the item first.", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();
                    //Update First Name
                    var query2 = conn.Query<Appointments>("UPDATE Appointments SET FirstName = '" + newFirstName + "' WHERE AppointmentID ='" + selectedItem + "'");
                    //Update Last Name
                    var query3 = conn.Query<Appointments>("UPDATE Appointments SET LastName = '" + newLastName + "' WHERE AppointmentID ='" + selectedItem + "'");
                    //Update Date of Appointment
                    var query4 = conn.Query<Appointments>("UPDATE Appointments SET DateOfAppointment = '" + DOAstring + "' WHERE AppointmentID ='" + selectedItem + "'");
                    //Update Time of Appointment 
                    var query5 = conn.Query<Appointments>("UPDATE Appointments SET TimeOfAppointmant = '" + newTOA + "' WHERE AppointmentID ='" + selectedItem + "'");

                    //This will deselect the item and prevent updating last selected item.
                    AppointmentsView.ItemsSource = query1.ToList(); 
                    selectedItem = ""; 
                    ClearFields();
                }
            }
            catch (NullReferenceException)
            {
                //Prevents a crash from pressing update button while no item selected.
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Have you selected an item?", "Oh dear..!");
                await dialog.ShowAsync();
            }
        }

        // Update the Fields
        private async void ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AppointmentsView.SelectedItem != null)
                    if (((Appointments)AppointmentsView.SelectedItem).AppointmentID.ToString() != null)
                    {
                        selectedItem = ((Appointments)AppointmentsView.SelectedItem).AppointmentID.ToString();
                  
                        FirstNametxtBox.Text = ((Appointments)AppointmentsView.SelectedItem).FirstName;
                        LastNametxtBox.Text = ((Appointments)AppointmentsView.SelectedItem).LastName;

                        DOAdatePicker.Date = DateTime.Parse(((Appointments)AppointmentsView.SelectedItem).DateOfAppointment.ToString());
                        TOAtimePicker.Time = TimeSpan.Parse(((Appointments)AppointmentsView.SelectedItem).TimeOfAppointmant.ToString());
                    }
            }
            catch (NullReferenceException)
            {
                // Prevents a crash from pressing update button while no item selected.
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oh dear..!");
                await dialog.ShowAsync();
            }

        }
    }
}
