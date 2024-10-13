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
        public static UserPage Instance { get; private set; }
        public static Dictionary<string, Uri> PageDefinitions;
        static readonly int[] staffLevels = { 4, 5 };
        static readonly int[] orgLevels = { 1, 2, 3 };
        public UserPage()
        {
            InitializeComponent();
            Instance = this;
            PageDefinitions = new Dictionary<string, Uri>()
            {
                ["Organizer"] = new Uri("Pages/Organizer.xaml", UriKind.RelativeOrAbsolute),
                ["Betting"] = new Uri("Pages/Betting.xaml", UriKind.RelativeOrAbsolute),
                ["Admin"] = new Uri("Pages/Admin.xaml", UriKind.RelativeOrAbsolute),
                ["MyBets"] = new Uri("Pages/MyBets.xaml", UriKind.RelativeOrAbsolute),
                ["Settings"] = new Uri("Pages/Settings.xaml", UriKind.RelativeOrAbsolute)
            };
            updateRepresentedData();
        }
        public void updateRepresentedData() 
        {
            Permission highestPermission = User.Session.HighestPermission();
            if (highestPermission != null)
            {
                mdpi_UserCreds.Kind = highestPermission.Role.RoleIcon;
            }
            if (!User.Session.HasPermissibilityLevel(staffLevels))
            {
                lviAdminPanelBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                lviAdminPanelBtn.Visibility = Visibility.Visible;
            }
            if (!User.Session.HasPermissibilityLevel(orgLevels))
            {
                lviOrganize.Visibility = Visibility.Collapsed;
            }
            else
            {
                lviOrganize.Visibility = Visibility.Visible;
            }
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
            spPages.Source = PageDefinitions["Settings"];
        }

        private void lviLogout_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Auth auth = new Auth();
            User.Session = null;
            auth.Show();
            this.Close();
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
    }
}
