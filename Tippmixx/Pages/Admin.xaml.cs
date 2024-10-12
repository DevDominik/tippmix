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
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        static User selectedUser = null;
        public Admin()
        {
            InitializeComponent();
            dtgFelhasznalok.ItemsSource = DataHandler.GetAllBettors(null);
        }

        private void lviPwAct_Details_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement baseElement = sender as FrameworkElement;
            selectedUser = baseElement.DataContext as User;
            grdUserListing.Visibility = Visibility.Collapsed;
            grdUserDetails.Visibility = Visibility.Visible;
            mdpi_IsActive.Kind = selectedUser.UserStatusAsIcon;
            mdpi_DetailsToggleAccess.Kind = selectedUser.AllowAccessAsIcon;
            tbDetailsToggleAccess.Text = selectedUser.AllowAccessAsString;
            tbUsername.Text = selectedUser.Username;
            tbDetailsUsername.Text = selectedUser.Username;
            tbDetailsCash.Text = selectedUser.Balance.ToString();
            if (!User.Session.HasPermissibilityLevel(5))
            {
                tbDetailsRolesTitleDisplay.Visibility = Visibility.Collapsed;
                dtgDetailsRoles.Visibility = Visibility.Collapsed;
                dtgDetailsRoles.ItemsSource = null;
            }
            else
            {
                tbDetailsRolesTitleDisplay.Visibility = Visibility.Visible;
                dtgDetailsRoles.Visibility = Visibility.Visible;
                dtgDetailsRoles.ItemsSource = selectedUser.Permissions;
            }
        }

        private void lviPwAct_ToggleActive_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            User user;
            if (selectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                user = baseElement.DataContext as User;
            }
            else
            {
                user = selectedUser;
            }
            if (MessageBox.Show($"Are you sure you want to {user.AllowAccessAsString.ToLower()} user {user.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                user.IsActive = !user.IsActive;
                mdpi_IsActive.Kind = user.UserStatusAsIcon;
                mdpi_DetailsToggleAccess.Kind = user.AllowAccessAsIcon;
                tbDetailsToggleAccess.Text = user.AllowAccessAsString;
                MessageBox.Show($"User {user.Username}'s access has been updated.", "Admin Panel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dtgFelhasznalok.Items.Refresh();
        }

        private void lviPwAct_CashReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            User user;
            if (selectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                user = baseElement.DataContext as User;
            }
            else
            {
                user = selectedUser;
            }
            if (MessageBox.Show($"Are you sure you want to reset the balance of user {user.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                user.Balance = 0;
                tbDetailsCash.Text = "0";
                MessageBox.Show($"User {user.Username}'s balance has been reset.", "Admin Panel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dtgFelhasznalok.Items.Refresh();
        }

        private void lviPwAct_PasswordReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            User user;
            if (selectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                user = baseElement.DataContext as User;
            }
            else
            {
                user = selectedUser;
            }
            if (MessageBox.Show($"Are you sure you want to reset the password of user {user.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                string newPassword = Misc.RandomString(8);
                user.Password = EasyEncryption.SHA.ComputeSHA256Hash(newPassword);
                MessageBox.Show($"User {user.Username}'s password has been reset.", "Admin Panel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dtgFelhasznalok.Items.Refresh();
        }


        private void dtgFelhasznalok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tbSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            dtgFelhasznalok.ItemsSource = DataHandler.GetAllBettors(tbSearchBar.Text);
        }

        private void lviReturnFromDetails_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            selectedUser = null;
            grdUserDetails.Visibility = Visibility.Collapsed;
            grdUserListing.Visibility = Visibility.Visible;
            dtgDetailsRoles.ItemsSource = null;
            dtgFelhasznalok.Items.Refresh();
        }

        private void lviUserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lviMessaging_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lviPwAct_UsernameReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            User user;
            if (selectedUser == null)
            {
                FrameworkElement baseElement = sender as FrameworkElement;
                user = baseElement.DataContext as User;
            }
            else
            {
                user = selectedUser;
            }
            if (MessageBox.Show($"Are you sure you want to reset the username of user {user.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                selectedUser.Username = $"tippmix_{Misc.RandomString(12)}";
                tbUsername.Text = tbDetailsUsername.Text;
            }
        }

        private void tbDetailsCash_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (selectedUser == null) { return; }
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void tbDetailsUsername_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (selectedUser == null)
            {
                return;
            }

            TextBox textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !Regex.IsMatch(e.Text, "^[a-z0-9_]+$") || newText.Length > 20 || newText.Length < 4;
        }

        private void lviUpdateUser_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to update user {selectedUser.Username}?", "Admin Panel", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                MessageBox.Show($"User {selectedUser.Username} has been updated.", "Admin Panel", MessageBoxButton.OK, MessageBoxImage.Information);
                selectedUser.Username = tbDetailsUsername.Text;
                tbUsername.Text = tbDetailsUsername.Text;
                selectedUser.Balance = Convert.ToInt32(tbDetailsCash.Text);
                dtgFelhasznalok.Items.Refresh();
            }
        }
    }
}
