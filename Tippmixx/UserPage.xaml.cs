using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Tippmixx
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class UserPage : Window
    {
        public static Dictionary<string, Uri> PageDefinitions;

        public UserPage()
        {
            InitializeComponent();
            PageDefinitions = new Dictionary<string, Uri>()
            {
                ["Betting"] = new Uri("Pages/Betting.xaml", UriKind.RelativeOrAbsolute),
                ["Organizer"] = new Uri("Pages/Organizer.xaml", UriKind.RelativeOrAbsolute),
                ["Admin"] = new Uri("Pages/Admin.xaml", UriKind.RelativeOrAbsolute)
            };

            Role.BuildRoles();
            tbUsername.Text = Session.Username;
            tbBalance.Text = Session.Balance.ToString("C"); // Display balance as currency
        }

        private void lviBettingPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = PageDefinitions["Betting"];
        }

        private void lviMyBets_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Implement My Bets navigation here if needed
        }

        private void lviOrganize_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = PageDefinitions["Organizer"];
        }

        private void lviAdminPanelBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = PageDefinitions["Admin"];
        }

        private void lviSettings_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Implement Settings navigation here if needed
        }

        private void lviLogout_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Implement Logout logic here
        }

        private void lviHome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Implement Home navigation here if needed
        }

        private void spPages_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!spPages.CanGoBack && !spPages.CanGoForward)
            {
                return;
            }

            var entry = spPages.RemoveBackEntry();
            while (entry != null)
            {
                entry = this.spPages.RemoveBackEntry();
            }
        }

        // Call this method to update the user's balance displayed on the UI
        public void UpdateBalance(decimal newBalance)
        {
            Session.Balance = (int)newBalance; // Update the balance in the session
            tbBalance.Text = newBalance.ToString("C"); // Update the balance display
        }
    }
}
