using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
using ZstdSharp.Unsafe;

namespace Tippmixx.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        static bool passwordInReveal = false;
        public Settings()
        {
            InitializeComponent();
            tbUsername.Text = User.Session.Username;
            tbEmail.Text = User.Session.Email;
        }

        private void tbUsername_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, "^[a-z0-9_]+$") || e.Text.Length > 20 || e.Text.Length < 4)
            {
                e.Handled = true;
            }
        }

        private void tbPassword_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            tbPasswordVis.Text = tbPassword.Password + e.Text;
        }

        private void lviInputKey_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to use this role key? This will give you a new authorization level.", "Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bool success = DataHandler.UseRoleKey(User.Session, tbRoleKey.Text);
                if (!success) 
                {
                    MessageBox.Show($"Key {tbRoleKey.Text} is not available or used up already.", "Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
                    tbRoleKey.Text = "";
                    return;
                }
                MessageBox.Show($"Successfully activated the role key.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                tbRoleKey.Text = "";
                UserPage.Instance.updateRepresentedData();
            }
        }

        private void lviUpdateUser_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to update your data?", "Settings", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Regex hasNumber = new Regex(@"[0-9]+");
                Regex hasUpperChar = new Regex(@"[A-Z]+");
                Regex hasMinimum8Chars = new Regex(@".{8,}");
                Regex isEmailCorrect = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

                bool isValidated = hasNumber.IsMatch(tbPassword.Password) && hasUpperChar.IsMatch(tbPassword.Password) && hasMinimum8Chars.IsMatch(tbPassword.Password);
                if (!isValidated)
                {
                    MessageBox.Show($"Your password does not meet the criterias (atleast one capital letter, atleast one number and has to have atleast 8 characters).", "Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                isValidated = isEmailCorrect.IsMatch(tbEmail.Text);
                if (!isValidated) 
                {
                    MessageBox.Show($"Your email does not meet the criterias (example: example@example.com).", "Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MessageBox.Show($"Your data has been updated.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                User.Session.Username = tbUsername.Text;
                User.Session.Email = tbEmail.Text;
                User.Session.Password = EasyEncryption.SHA.ComputeSHA256Hash(tbPassword.Password);
            }
        }

        private void lviTogglePasswordVisibility_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (passwordInReveal)
            {
                tbPasswordReveal.Text = "Reveal Password";
                mdpi_PasswordReveal.Kind = PackIconKind.Eye;
                splPassword.Visibility = Visibility.Visible;
                splPasswordVis.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbPasswordReveal.Text = "Hide Password";
                mdpi_PasswordReveal.Kind = PackIconKind.EyeClosed;
                splPasswordVis.Visibility = Visibility.Visible;
                splPassword.Visibility = Visibility.Collapsed;
            }
            passwordInReveal = !passwordInReveal;
        }

        private void tbPasswordVis_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            tbPassword.Password = tbPasswordVis.Text + e.Text;
        }
    }
}
