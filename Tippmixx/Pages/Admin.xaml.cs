using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        static User? selectedUser = null;
        static bool canPushChangesToDb = false;
        public Admin()
        {
            InitializeComponent();
            dtgFelhasznalok.ItemsSource = User.RefreshUserList(null);
        }

        private void lviPwAct_Details_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            canPushChangesToDb = false;
            FrameworkElement baseElement = sender as FrameworkElement;
            selectedUser = baseElement.DataContext as User;
            grdUserListing.Visibility = Visibility.Collapsed;
            grdUserDetails.Visibility = Visibility.Visible;
            tbUsername.Text = selectedUser.Username;
            tbDetailsUsername.Text = selectedUser.Username;
            tbDetailsCash.Text = selectedUser.Balance.ToString();
            if (!Permission.HasPermissibilityLevel(User.Session, 5))
            {
                tbDetailsRolesTitleDisplay.Visibility = Visibility.Collapsed;
                dtgDetailsRoles.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbDetailsRolesTitleDisplay.Visibility = Visibility.Visible;
                dtgDetailsRoles.Visibility = Visibility.Visible;
            }
            canPushChangesToDb = true;
        }

        private void lviPwAct_ToggleActive_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement baseElement = sender as FrameworkElement;
            User user = baseElement.DataContext as User;
            if (MessageBox.Show($"Are you sure you want to {user.AllowAccessAsString.ToLower()} user {user.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                user.IsActive = !user.IsActive;
            }
            dtgFelhasznalok.Items.Refresh();
        }

        private void lviPwAct_CashReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                User user = baseElement.DataContext as User;
                if (MessageBox.Show($"Are you sure you want to reset the balance of user {user.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    user.Balance = 0;
                }
            }
            else
            {
                if (MessageBox.Show($"Are you sure you want to reset the balance of user {selectedUser.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    canPushChangesToDb = false;
                    selectedUser.Balance = 0;
                    tbDetailsCash.Text = "0";
                    canPushChangesToDb = true;
                }
            }
        }

        private void lviPwAct_PasswordReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                User user = baseElement.DataContext as User;
                if (MessageBox.Show($"Are you sure you want to reset the balance of user {user.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    user.Balance = 0;
                }
            }
            else
            {
                if (MessageBox.Show($"Are you sure you want to reset the balance of user {selectedUser.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    canPushChangesToDb = false;
                    selectedUser.Balance = 0;
                    tbDetailsCash.Text = "0";
                    canPushChangesToDb = true;
                }
            }
        }

        private void lviPwAct_EmailReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lviMenuPointSwitch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void dtgFelhasznalok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            dtgFelhasznalok.ItemsSource = User.RefreshUserList(tbSearchBar.Text);
        }

        private void lviReturnFromDetails_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
