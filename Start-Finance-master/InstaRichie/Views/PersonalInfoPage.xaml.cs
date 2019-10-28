using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();

            DOB.Date = DateTime.Now;
        }

        public void UpdateList()
        {
            var result = conn.Table<PersonalInfo>(); // Connects to the Table
            PersonListView.ItemsSource = result.ToList(); // Return all the records from the Table
        }

        private async void AddPerson(object sender, RoutedEventArgs e)
        {
            try
            {
                string CDay = DOB.Date.Value.Day.ToString();
                string CMonth = DOB.Date.Value.Month.ToString();
                string CYear = DOB.Date.Value.Year.ToString();
                string FinalDate = "" + CDay + "/" + CMonth + "/" + CYear;
                string Choice;

                if (Male.IsChecked == true)
                {
                    Choice = "Male";
                }

                else
                {
                    Choice = "Female";
                }

                if (FirstNameText.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No First Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }

                else
                {
                    conn.Insert(new PersonalInfo()
                    {
                        
                        FirstName = FirstNameText.Text,
                        LastName = LastNameText.Text,
                        Email = EmailText.Text,
                        Phone = PhoneText.Text,
                        DOB = FinalDate,
                        Gender = Choice

                });
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A Similar Asset Nane already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }


        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();

            /// Refresh Data
            var query = conn.Table<PersonalInfo>();
            PersonListView.ItemsSource = query.ToList();
        }

        private async void DeleteAccout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PersonSelection = ((PersonalInfo)PersonListView.SelectedItem).FirstName;
                if (PersonSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<PersonalInfo>();
                    var query1 = conn.Table<PersonalInfo>();
                    var query3 = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE FirstName ='" + PersonSelection + "'");
                    PersonListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected person 
                PersonalInfo selectedPerson = (PersonalInfo)PersonListView.SelectedItem;
                if (selectedPerson == null)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    // Update the fields of the selected person
                    FirstNameText.Text = selectedPerson.FirstName;
                    LastNameText.Text = selectedPerson.LastName;
                    DOB.Date = DateTime.Parse(selectedPerson.DOB);
                    
                    EmailText.Text = selectedPerson.Email;
                    PhoneText.Text = selectedPerson.Phone.ToString();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected person
                PersonalInfo selectedPerson = (PersonalInfo)PersonListView.SelectedItem;
                if (selectedPerson == null)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    // Check if First Name and Last Name are filled
                    if (FirstNameText.Text.Length == 0 || LastNameText.Text.Length == 0)
                    {
                        MessageDialog dialog = new MessageDialog("No value entered for First Name or Last Name", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        string CDay = DOB.Date.Value.Day.ToString();
                        string CMonth = DOB.Date.Value.Month.ToString();
                        string CYear = DOB.Date.Value.Year.ToString();
                        string FinalDate = "" + CDay + "/" + CMonth + "/" + CYear;
                        string Choice;

                        if (Male.IsChecked == true)
                        {
                            Choice = "Male";
                        }

                        else
                        {
                            Choice = "Female";
                        }

                        // Update the selected person info
                        selectedPerson.FirstName = FirstNameText.Text;
                        selectedPerson.LastName = LastNameText.Text;
                        selectedPerson.DOB = FinalDate;
                        selectedPerson.Gender = Choice;
                        selectedPerson.Email = EmailText.Text;
                        selectedPerson.Phone = PhoneText.Text;

                        // Send the update to the Database
                        conn.Update(selectedPerson);

                        UpdateList(); // Update the list and page
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Phone number or entered an invalid Phone number", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Database error: " + ex.Message, "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Generic error: " + ex.Message, "Oops..!");
                    await dialog.ShowAsync();
                }
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

      
    }
}
