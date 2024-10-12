using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace Tippmixx
{
    public partial class OrganizerPage : Page
    {
        public OrganizerPage()
        {
            InitializeComponent();
            LoadEvents();
        }

        private void LoadEvents(string searchTerm = "")
        {
            ObservableCollection<Event> eventList = DataHandler.RefreshEventList(searchTerm);
            dtgEvents.ItemsSource = eventList;
        }
        private void tbOrganizerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadEvents(tbOrganizerSearch.Text);
        }

        private void dtgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
