using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;

namespace Tippmixx
{
    public partial class MyBets : Page
    {
        public MyBets()
        {
            InitializeComponent();
            LoadBets();
        }

        private void LoadBets()
        {
            try
            {
                // Connection string
                string connString = "Server=localhost;Database=tippmix;User ID=root;Password=;";
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    // Query to retrieve bets for the logged-in user
                    string query = "SELECT Bets.BetsID, Events.EventName, Bets.Amount, Bets.Odds, Bets.BetDate " +
                                   "FROM Bets " +
                                   "JOIN Events ON Bets.EventID = Events.EventID " +
                                   "WHERE Bets.BettorsID = @BettorsID";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BettorsID", User.Session.Id); // Assuming Session.ID holds the logged-in user's ID
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Set the DataGrid's ItemsSource to the retrieved data
                        BetsDataGrid.ItemsSource = dataTable.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading bets: {ex.Message}");
            }
        }

        
    }
}
