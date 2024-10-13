using System;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Tippmixx
{
    public partial class Betting : Page
    {
        private ObservableCollection<Event> _eventsList;
        private Event _selectedEvent;
        private Random _random;
        private int _bettorId;
        private float _generatedOdds;

        public Betting()
        {
            InitializeComponent();
            DataContext = this;
            _random = new Random();
            _bettorId = User.Session.Id;
            _eventsList = DataHandler.RefreshEventList();
            EventComboBox.ItemsSource = _eventsList;
        }
        
        private void PlaceBetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedEvent != null)
                {
                    int amount = int.Parse(AmountTextBox.Text);
                    if (amount > User.Session.Balance)
                    {
                        MessageBox.Show("Insufficient balance to place this bet.");
                        return;
                    }
                    //DeductBalance(_bettorId, amount);
                    User.Session.Balance -= amount;
                    DataHandler.PlaceBet(User.Session, _selectedEvent.EventID, _generatedOdds, amount);
                    ConfirmationTextBlock.Text = $"Bet placed successfully for Event: {_selectedEvent.EventName} with odds: {_generatedOdds}. Amount: {amount:C} deducted from your balance.";
                    AmountTextBox.Clear();
                    EventComboBox.SelectedIndex = -1;
                    OddsTextBlock.Text = string.Empty;
                    UserPage.Instance.updateRepresentedData();
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
    private void EventComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EventComboBox.SelectedItem is Event selectedEvent)
            {
                _selectedEvent = selectedEvent;
                _generatedOdds = (float)(_random.NextDouble() * (5.0 - 1.5) + 1.5);
                OddsTextBlock.Text = _generatedOdds.ToString("0.00");
            }
        }
    }
}
