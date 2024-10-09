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
        public Admin()
        {
            InitializeComponent();
            dtgFelhasznalok.ItemsSource = User.RefreshUserList();
        }

        private void lviPwAct_Details_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

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

        }

        private void lviPwAct_PasswordReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

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
    }
}
