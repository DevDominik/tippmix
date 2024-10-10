using System;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient; // For MySQL commands
using System.Windows;
using System.Windows.Controls;

namespace Tippmixx
{
    /// <summary>
    /// Interaction logic for Betting.xaml
    /// </summary>
    public partial class Betting : Page
    {
        private ObservableCollection<Event> _eventsList;
        private Event _selectedEvent;
        private Random _random;
        private int _bettorId;
        private float _generatedOdds;
        private decimal _balance; // Bettor's balance
        private UserPage _userPage; // Reference to UserPage

        private string connectionString = "Server=localhost;Database=tippmix;User ID=root;Password=;"; // MySQL connection string

        public Betting() : this(null) { }

        public Betting(UserPage userPage)
        {
            InitializeComponent();
            DataContext = this;
            _userPage = userPage; // Assign the user page reference

            // Initialize random generator and fetch bettor ID
            _random = new Random();
            _bettorId = User.Session.Id; // Assuming bettor ID is stored in Session class

            // Fetch bettor's balance from the database
            _balance = GetBettorBalance(_bettorId);
            _eventsList = EventManager.RefreshEventList();
            EventComboBox.ItemsSource = _eventsList; // Bind events to ComboBox
        }

        // Fetch the bettor's balance from the database
        private decimal GetBettorBalance(int bettorId)
        {
            decimal balance = 0;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Balance FROM Bettors WHERE BettorsID = @bettorId";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@bettorId", bettorId);
                    balance = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            return balance;
        }

        // Update the balance display on the UI
       
        // Deduct the amount from bettor's balance and update in the database
        private void DeductBalance(int bettorId, int amount)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Bettors SET Balance = Balance - @amount WHERE BettorsID = @bettorId";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@bettorId", bettorId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Event handler for placing a bet
        private void PlaceBetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedEvent != null)
                {
                    int amount = int.Parse(AmountTextBox.Text);

                    // Check if the bettor has enough balance
                    if (amount > _balance)
                    {
                        MessageBox.Show("Insufficient balance to place this bet.");
                        return;
                    }

                    // Deduct the balance before placing the bet
                    DeductBalance(_bettorId, amount);

                    // Update balance in the UI
                    _balance -= amount;

                    // Call the method to update the balance on the UserPage
                    _userPage.UpdateBalance(_balance); // Update balance in UserPage

                    // Place the bet using the EventManager
                    EventManager.PlaceBet(_bettorId, _selectedEvent.EventID, _generatedOdds, amount);

                    // Display confirmation message
                    ConfirmationTextBlock.Text = $"Bet placed successfully for Event: {_selectedEvent.EventName} with odds: {_generatedOdds}. Amount: {amount:C} deducted from your balance.";

                    // Allow placing another bet without switching pages
                    AmountTextBox.Clear();
                    EventComboBox.SelectedIndex = -1;
                    OddsTextBlock.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Please select an event before placing a bet.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error placing bet: {ex.Message}");
            }
        }

    // Event handler for when a new event is selected
    private void EventComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventComboBox.SelectedItem is Event selectedEvent)
            {
                _selectedEvent = selectedEvent;

                // Generate random odds between 1.5 and 5.0
                _generatedOdds = (float)(_random.NextDouble() * (5.0 - 1.5) + 1.5);
                OddsTextBlock.Text = _generatedOdds.ToString("0.00");
            }
        }
    }
}
