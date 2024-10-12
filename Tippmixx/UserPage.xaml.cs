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
                ["Organizer"] = new Uri("Pages/Organizer.xaml", UriKind.RelativeOrAbsolute),
                ["Betting"] = new Uri("Pages/Betting.xaml", UriKind.RelativeOrAbsolute),
                ["Admin"] = new Uri("Pages/Admin.xaml", UriKind.RelativeOrAbsolute),
                ["MyBets"] = new Uri("Pages/MyBets.xaml", UriKind.RelativeOrAbsolute)
            };
            tbUsername.Text = User.Session.Username;
            tbBalance.Text = User.Session.Balance.ToString("C");
        }

        private void lviBettingPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = PageDefinitions["Betting"];
        }

        private void lviMyBets_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = PageDefinitions["MyBets"];

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
            
        }

        private void lviLogout_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Auth auth = new Auth();
            auth.Show();
            this.Close();
        }

        private void lviHome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
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

        
        public void UpdateBalance(decimal newBalance)
        {
            User.Session.Balance = (int)newBalance; 
            tbBalance.Text = newBalance.ToString("C"); 
        }
    }
}
