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
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();

            lbUsername.Content = Session.Username;
            lbEmail.Content = Session.Email;
            lbBalance.Content = Session.Balance;
            lbIsactive.Content = Session.IsActive ? "Aktív" : "Inaktív";
            lbClass.Content = Session.Perm;
        }

        private void lviBettingPage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Betting betpage = new Betting();
            spPages.Children.Add(betpage);
        }

        private void lbiMyBets_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lbiOrganize_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lbiAdminPanelBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
