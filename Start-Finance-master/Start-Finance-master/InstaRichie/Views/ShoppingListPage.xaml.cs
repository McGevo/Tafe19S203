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
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "FindData.sqlite");

        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        public void Results()
        {
            conn.CreateTable<ShoppingItem>();
            var sqlQuery = conn.Table<ShoppingItem>();
            ShoppingListView.ItemsSource = sqlQuery.ToList();
        }

        private async void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            ShoppingItem selection = (ShoppingItem)ShoppingListView.SelectedItem;
            try
            {
                if (selection == null)
                {
                    MessageDialog dialog = new MessageDialog("Item not selected");
                    await dialog.ShowAsync();
                }
                else
                {
                    ShopNameTB.Text = selection.ShopName;
                    ItemNameTB.Text = selection.NameOfItem;
                    DateTB.Date = selection.ShoppingDate;
                    QuoteTB.Text = selection.PriceQuoted.ToString();

                }
                Results();
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Item not selected");
                await dialog.ShowAsync();
            }
        }

        private async void AddShopItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ShopNameTB.Text.ToString() == "" || ItemNameTB.Text.ToString() == "" || QuoteTB.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Please enter values in all text boxes");
                    await dialog.ShowAsync();
                }
                else
                {

                    DateTime TempDate = DateTime.Parse(DateTB.Date.ToString());
                    double TempQuote = Convert.ToDouble(QuoteTB.Text);
                    conn.CreateTable<ShoppingItem>();
                    conn.Insert(new ShoppingItem
                    {
                        NameOfItem = ItemNameTB.Text.ToString(),
                        ShopName = ShopNameTB.Text.ToString(),
                        ShoppingDate = TempDate,
                        PriceQuoted = TempQuote
                    });

                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You entered invalid values in date and/or quote");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Item Name already exists, try a different name");
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("How did you mess this up?");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string accSelection = ((ShoppingItem)ShoppingListView.SelectedItem).NameOfItem;
                if (accSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Item not selected");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ShoppingItem>();
                    var query1 = conn.Table<ShoppingItem>();
                    var query2 = conn.Query<ShoppingItem>("DELETE FROM ShoppingItem WHERE NameOfItem ='" + accSelection + "'");
                    ShoppingListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Item not selected");
                await dialog.ShowAsync();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        public void UpdateList()
        {
            var result = conn.Table<ShoppingItem>();
            ShoppingListView.ItemsSource = result.ToList();
        }

        private async void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShoppingItem selection = (ShoppingItem)ShoppingListView.SelectedItem;
                if (selection == null)
                {
                    MessageDialog dialog = new MessageDialog("Item not selected");
                    await dialog.ShowAsync();
                }
                else
                {
                    if (ShopNameTB.Text.Length == 0 || ItemNameTB.Text.Length == 0 || QuoteTB.Text.Length == 0)
                    {
                        MessageDialog dialog = new MessageDialog("Please fill all values in");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        string sDay = DateTB.Date.Day.ToString();
                        string sMonth = DateTB.Date.Month.ToString();
                        string sYear = DateTB.Date.Year.ToString();
                        string finalDate = sDay + "/" + sMonth + "/" + sYear;

                        selection.ShopName = ShopNameTB.Text;
                        selection.NameOfItem = ItemNameTB.Text;
                        selection.ShoppingDate = DateTime.Parse(finalDate);
                        selection.PriceQuoted = double.Parse(QuoteTB.Text);

                        conn.Update(selection);
                        UpdateList();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You entered an invalid quote");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Database error: " + ex.Message);
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Generic error: " + ex.Message);
                    await dialog.ShowAsync();
                }
            }
        }
    }
}
