using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Tippmixx
{
    /// <summary>
    /// Interaction logic for OrganizerPage.xaml
    /// </summary>
    public partial class OrganizerPage : Page
    {
        public OrganizerPage()
        {
            InitializeComponent();
            LoadEvents();
        }

        // Method to load events into DataGrid
        private void LoadEvents(string searchTerm = "")
        {
            // Fetch events using EventManager and display in DataGrid
            ObservableCollection<Event> eventList = EventManager.RefreshEventList(searchTerm);
            dtgEvents.ItemsSource = eventList;
        }

        // Event handler for when the search box text changes
        private void tbOrganizerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Re-load the events based on the search term entered
            LoadEvents(tbOrganizerSearch.Text);
        }

        private void dtgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // You can handle selection logic here if needed.
        }
    }
}
