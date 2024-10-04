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
        private bool AuthenticateUser(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(dbConnectionString))
            {
                conn.Open();
                string query = @"
                SELECT Username, Email, Balance, IsActive 
                FROM Bettors 
                WHERE Username = @username AND Password = @password";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username.ToLower());
                    cmd.Parameters.AddWithValue("@password", EasyEncryption.SHA.ComputeSHA256Hash(password));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Session.Username = reader["Username"].ToString();
                            Session.Password = EasyEncryption.SHA.ComputeSHA256Hash(password);
                            Session.Email = reader["Email"].ToString();
                            Session.Balance = Convert.ToInt32(reader["Balance"]);
                            Session.IsActive = Convert.ToBoolean(reader["IsActive"]);
                            //Session.Perm.Add();

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }


        private bool RegisterUser(string username, string password, string email, int balance)
        {
            using (MySqlConnection conn = new MySqlConnection(dbConnectionString))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(1) FROM Bettors WHERE Username = @username OR Email = @Email";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@username", username.ToLower());
                    checkCmd.Parameters.AddWithValue("@Email", email.ToLower());

                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        MessageBox.Show("Username or email already exists!");
                        return false;
                    }
                }

                string insertQuery = @"
            INSERT INTO Bettors (Username, Balance, Email, Password, JoinDate, IsActive)
            VALUES (@username, @balance, @Email, @password, @joinDate, @isActive)";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username.ToLower());
                    cmd.Parameters.AddWithValue("@password", EasyEncryption.SHA.ComputeSHA256Hash(password));
                    cmd.Parameters.AddWithValue("@Email", email.ToLower());
                    cmd.Parameters.AddWithValue("@balance", balance);
                    cmd.Parameters.AddWithValue("@joinDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@isActive", true);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Registration successful!");
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Registration failed. Please try again.");
                        return false;
                    }
                }
            }
        }

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
                    MessageBox.Show("Login successful!");

                    UserPage uw = new UserPage();
                    this.Close();
                    uw.Show();
                }
                else
                {
                    MessageBox.Show("Invalid user/pass");
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
                    MessageBox.Show("Nem egyezik a 2 jelszo");
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
                tbStateBtn.Text = "Register";
                mdpi_Action.Kind = PackIconKind.Register;
                return;
            }
            splEmail.Visibility = Visibility.Collapsed;
            splPasswordRep.Visibility = Visibility.Collapsed;
            tbStateBtn.Text = "Log in";
            tbState.Text = "Not a member yet? Sign up";
            mdpi_Action.Kind = PackIconKind.Login;
            isLogin = true;
        }
    }
}
