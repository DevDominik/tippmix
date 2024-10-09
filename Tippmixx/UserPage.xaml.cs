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
        public UserPage()
        {
            InitializeComponent();

            tbUsername.Text = Session.Username;
            tbBalance.Text = Session.Balance.ToString();
        }

        private void lviBettingPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = new Uri("Pages/Betting.xaml", UriKind.RelativeOrAbsolute);
        }

        private void lviMyBets_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lviOrganize_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = new Uri("Pages/Organizer.xaml", UriKind.RelativeOrAbsolute);

        }

        private void lviAdminPanelBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            spPages.Source = new Uri("Pages/Admin.xaml", UriKind.RelativeOrAbsolute);
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

        }
    }
}
