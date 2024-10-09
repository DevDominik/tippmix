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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            dtgEvents.ItemsSource = EventManager.RefreshEventList();
            EventManager.CreateEvent("Basketball Tournament", new DateTime(2024, 11, 1), "Sports", "Arena B");
            //EventManager.PlaceBet(1, 2, 1.5f, 100); // BettorID = 1, EventID = 2, Odds = 1.5, Amount = 100
            //ObservableCollection<Bet> bets = EventManager.GetBetsByEventId(2); // Replace with the actual EventID

        }

        private void dtgEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbOrganizerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            dtgEvents.ItemsSource = EventManager.RefreshEventList(tbOrganizerSearch.Text);
        }
    }
}
