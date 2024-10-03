using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
namespace Tippmixx
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
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
                            Session.Class = Session.Username == "admin" ? "Admin" : Session.Username == "organizer" ? "Organizer" : "User";

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


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = tbUsername.Text.ToLower();
            string password = tbPassword.Text;

            if (AuthenticateUser(username, EasyEncryption.SHA.ComputeSHA256Hash(password)))
            {
                MessageBox.Show("Login successful!");

                User uw = new User();
                this.Close();
                uw.Show();
            }
            else
            {
                //tbResult.Text = "Invalid username or password.";
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterUser(tbUsername.Text.ToLower(), EasyEncryption.SHA.ComputeSHA256Hash(tbPassword.Text), tbEmail.Text, 10000);
        }

        private void lviLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string username = tbUsername.Text.ToLower();
            string password = tbPassword.Text;

            if (AuthenticateUser(username, EasyEncryption.SHA.ComputeSHA256Hash(password)))
            {
                MessageBox.Show("Login successful!");

                User uw = new User();
                this.Close();
                uw.Show();
            }
            else
            {
                //tbResult.Text = "Invalid username or password.";
            }
        }

        private void lviAction_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
