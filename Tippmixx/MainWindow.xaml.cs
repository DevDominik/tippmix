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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EasyEncryption;
using Microsoft.Data.Sqlite;

namespace Tippmixx
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string pw = "asd123";

            var encrypted = EasyEncryption.SHA.ComputeSHA256Hash("pw");
            MessageBox.Show(encrypted);
        }
        private bool AuthenticateUser(string username, string password)
        {
            string dbConnectionString = "Data Source=bets.db;Version=3;";
            using (SQLiteConnection conn = new SQLiteConnection(dbConnectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM Bettors WHERE Username = @username AND Password = @password";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int result = Convert.ToInt32(cmd.ExecuteScalar());

                    return result == 1;
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = ;
            string password = ;

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!");
            }
            else
            {
                MessageTextBlock.Text = "Invalid username or password.";
            }
        }

    }
}
