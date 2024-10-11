using MySql.Data.MySqlClient;
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
using static System.Net.Mime.MediaTypeNames;
using EasyEncryption;
using MaterialDesignThemes.Wpf;
namespace Tippmixx
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Auth : Window
    {
        private static bool isLogin = true;
        public Auth()
        {
            InitializeComponent();
        }

        public readonly string dbConnectionString = "Server=localhost;Database=tippmix;User ID=root;Password=;";
       


      
        private void lviAction_Selected(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.ToLower();
            string password = tbPassword.Password;
            string pwrepeat = tbPasswordRep.Password;
            string email = tbEmail.Text.ToLower();
            if (isLogin == true)
            {

                if (AuthenticateUser(username, password))
                {
                    if (!User.Session.IsActive)
                    {
                        MessageBox.Show("Your account has been deactivated. You may not sign in with this account until a staff member reactivates your account.", "Auth", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    MessageBox.Show("Login successful.", "Auth", MessageBoxButton.OK, MessageBoxImage.Information);

                    UserPage uw = new UserPage();
                    this.Close();
                    uw.Show();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Auth", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                if (password == pwrepeat)
                {
                    RegisterUser(username, password, email, 10000);
                }
                else
                {
                    MessageBox.Show("The two passwords do not match.", "Auth", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void lviSwitch_Selected(object sender, RoutedEventArgs e)
        {
            if (isLogin)
            {
                splEmail.Visibility = Visibility.Visible;
                splPasswordRep.Visibility = Visibility.Visible;
                isLogin = false;
                tbState.Text = "Already a member? Sign in";
                tbStateBtn.Text = "Sign up";
                mdpi_Action.Kind = PackIconKind.Register;
                return;
            }
            splEmail.Visibility = Visibility.Collapsed;
            splPasswordRep.Visibility = Visibility.Collapsed;
            tbStateBtn.Text = "Sign in";
            tbState.Text = "Not a member yet? Sign up";
            mdpi_Action.Kind = PackIconKind.Login;
            isLogin = true;
        }
    }
}
