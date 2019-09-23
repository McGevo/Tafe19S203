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
    public sealed partial class ClientDetailsPage : Page
    {

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ClientDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Results();
        }
        public void Results()
        {
            conn.CreateTable<ClientDetails>();
            var query1 = conn.Table<ClientDetails>();
            ClientDetailsView.ItemsSource = query1.ToList();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            {
                try
                {
                    if (CDFirstName.Text.ToString() == "" || CDLastName.Text.ToString() == "" || phoneNumber.Text.ToString() == "" || CDCompanyName.Text.ToString() == "")
                    {
                        MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                        await dialog.ShowAsync();

                    }

                    else
                    {

                        conn.CreateTable<ClientDetails>();
                        conn.Insert(new ClientDetails
                        {
                            FirstName = CDFirstName.Text.ToString(),
                            LastName = CDLastName.Text.ToString(),
                            phoneNumber = phoneNumber.Text.ToString(),
                            CompanyName = CDCompanyName.Text.ToString()
                        });
                        // Creating table
                        Results();
                    }
                }
                catch (Exception ex)
                {
                    if (ex is FormatException)
                    {
                        MessageDialog dialog = new MessageDialog("entered an unsusable name or number", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else if (ex is SQLiteException)
                    {
                        MessageDialog dialog = new MessageDialog(" Name already exist, Try Different Name", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        /// no idea
                    }
                }
            }
        }

        private async void DeleteBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((ClientDetails)ClientDetailsView.SelectedItem).FirstName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ClientDetails>();
                    var query1 = conn.Table<ClientDetails>();
                    var query3 = conn.Query<ClientDetails>("DELETE FROM ClientDetails WHERE FirstName ='" + AccSelection + "'");
                    ClientDetailsView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }
        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {

            {
                try
                {
                    // Get the selected client
                    ClientDetails selectedclient = (ClientDetails)ClientDetailsView.SelectedItem;
                    if (selectedclient == null)
                    {
                        MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        // Update the fields of the selected person
                        CDFirstName.Text = selectedclient.FirstName;
                        CDLastName.Text = selectedclient.LastName;
                        CDCompanyName.Text = selectedclient.CompanyName;
                        phoneNumber.Text = selectedclient.phoneNumber.ToString();
                    }
                }
                catch (NullReferenceException)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
            }

        }
    }

}
