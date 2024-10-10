using System;
using System.Collections.ObjectModel;
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

        public Betting()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize random generator and fetch bettor ID
            _random = new Random();
            _bettorId = Session.ID; // Assuming bettor ID is stored in LoggedInUser class

            // Fetch the list of events from the database
            _eventsList = EventManager.RefreshEventList();
            EventComboBox.ItemsSource = _eventsList; // Bind events to ComboBox
        }

        // Event handler for placing a bet
        private void PlaceBetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedEvent != null)
                {
                    int amount = int.Parse(AmountTextBox.Text);

                    // Place the bet using the EventManager
                    EventManager.PlaceBet(_bettorId, _selectedEvent.EventID, _generatedOdds, amount);

                    // Display confirmation message
                    ConfirmationTextBlock.Text = $"Bet placed successfully for Event: {_selectedEvent.EventName} with odds: {_generatedOdds}";
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
