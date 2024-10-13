using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Organizer.xaml
    /// </summary>
    public partial class Organizer : Page
    {
        static Event selectedEvent = null;

        public Organizer()
        {
            InitializeComponent();
            dtgEvents.ItemsSource = DataHandler.RefreshEventList(null);
        }

        private void lviEventDetails_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement baseElement = sender as FrameworkElement;
            selectedEvent = baseElement.DataContext as Event;
            grdEventListing.Visibility = Visibility.Collapsed;
            grdEventDetails.Visibility = Visibility.Visible;
            //mdpi_EventStatus.Kind = selectedEvent.EventStatusAsIcon;
            //mdpi_ToggleEventStatus.Kind = selectedEvent.AllowBetsAsIcon;
            //tbToggleEventStatus.Text = selectedEvent.AllowBetsAsString;
            tbEventName.Text = selectedEvent.EventName;
            tbDetailsEventName.Text = selectedEvent.EventName;
            //tbDetailsMaxBet.Text = selectedEvent.MaxBet.ToString();
        }

        private void lviToggleEventStatus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Event evnt;
            if (selectedEvent == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                evnt = baseElement.DataContext as Event;
            }
            else
            {
                evnt = selectedEvent;
            }

            if (MessageBox.Show($"Are you sure you want to event {evnt.EventName}?", "Organizer Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //evnt.IsActive = !evnt.IsActive;
                //mdpi_EventStatus.Kind = evnt.EventStatusAsIcon;
                //mdpi_ToggleEventStatus.Kind = evnt.AllowBetsAsIcon;
                //tbToggleEventStatus.Text = evnt.AllowBetsAsString;
                MessageBox.Show($"Event {evnt.EventName}'s status has been updated.", "Organizer Panel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dtgEvents.Items.Refresh();
        }

        private void lviResetEventBets_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Event evnt;
            if (selectedEvent == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                evnt = baseElement.DataContext as Event;
            }
            else
            {
                evnt = selectedEvent;
            }

            if (MessageBox.Show($"Are you sure you want to reset the bets for event {evnt.EventName}?", "Organizer Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //evnt.ResetBets();
                MessageBox.Show($"All bets for event {evnt.EventName} have been reset.", "Organizer Panel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dtgEvents.Items.Refresh();
        }

        private void lviUpdateEvent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to update event {selectedEvent.EventName}?", "Organizer Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                MessageBox.Show($"Event {selectedEvent.EventName} has been updated.", "Organizer Panel", MessageBoxButton.OK, MessageBoxImage.Information);
                selectedEvent.EventName = tbDetailsEventName.Text;
                tbEventName.Text = tbDetailsEventName.Text;
                //selectedEvent.MaxBet = Convert.ToInt32(tbDetailsMaxBet.Text);
                dtgEvents.Items.Refresh();
            }
        }

        private void tbSearchEvents_TextChanged(object sender, TextChangedEventArgs e)
        {
            dtgEvents.ItemsSource = DataHandler.RefreshEventList(tbSearchEvents.Text);
        }

        private void lviReturnFromEventDetails_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedEvent = null;
            grdEventDetails.Visibility = Visibility.Collapsed;
            grdEventListing.Visibility = Visibility.Visible;
            dtgEvents.Items.Refresh();
        }
    }
}
