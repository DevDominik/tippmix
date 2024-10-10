using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            tbBalance.Text = Session.Balance.ToString();
        }

        private void lviBettingPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = PageDefinitions["Betting"];
        }

        private void lviMyBets_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

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

        private void lviBettingPage_Selected(object sender, RoutedEventArgs e)
        {

        }
    }
}
