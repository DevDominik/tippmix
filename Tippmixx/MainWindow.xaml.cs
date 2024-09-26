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
        }
        public readonly string dbConnectionString = "Data Source=bets.db;Version=3;";
        private bool AuthenticateUser(string username, string password)
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbConnectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(1) FROM Bettors WHERE Username = @username AND Password = @password";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", EasyEncryption.SHA.ComputeSHA256Hash(password));

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
                .Text = "Invalid username or password.";
            }
        }

        private void CreateDatabaseAndTables()
        {
            using (SQLiteConnection conn = new SQLiteConnection(dbConnectionString))
            {
                conn.Open();

                // Bettors tábla létrehozása
                string createBettorsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Bettors (
                        BettorsID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Balance INTEGER NOT NULL,
                        Email TEXT NOT NULL,
                        Password TEXT NOT NULL,
                        JoinDate DATE NOT NULL,
                        IsActive BOOLEAN NOT NULL DEFAULT 1
                    );";

                using (SQLiteCommand cmd = new SQLiteCommand(createBettorsTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Events tábla létrehozása
                string createEventsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Events (
                        EventID INTEGER PRIMARY KEY AUTOINCREMENT,
                        EventName TEXT NOT NULL,
                        EventDate DATE NOT NULL,
                        Category TEXT NOT NULL,
                        Location TEXT NOT NULL
                    );";

                using (SQLiteCommand cmd = new SQLiteCommand(createEventsTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Bets tábla létrehozása
                string createBetsTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Bets (
                        BetsID INTEGER PRIMARY KEY AUTOINCREMENT,
                        BetDate DATE NOT NULL,
                        Odds REAL NOT NULL,
                        Amount INTEGER NOT NULL,
                        BettorsID INTEGER NOT NULL,
                        EventID INTEGER NOT NULL,
                        Status BOOLEAN NOT NULL,
                        FOREIGN KEY (BettorsID) REFERENCES Bettors(BettorsID),
                        FOREIGN KEY (EventID) REFERENCES Events(EventID)
                    );";

                using (SQLiteCommand cmd = new SQLiteCommand(createBetsTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Teszt adatok beszúrása Bettors táblába
                string insertTestDataQuery = @"
                    INSERT INTO Bettors (Username, Balance, Email, Password, JoinDate, IsActive)
                    SELECT 'admin', 1000, 'admin@example.com', 'password', '2023-01-01', 1
                    WHERE NOT EXISTS (SELECT 1 FROM Bettors WHERE Username = 'admin');";

                using (SQLiteCommand cmd = new SQLiteCommand(insertTestDataQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
