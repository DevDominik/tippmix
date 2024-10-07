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
        public Admin()
        {
            InitializeComponent();
            dtgFelhasznalok.ItemsSource = User.RefreshUserList();
        }

        private void ListViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lviDetails_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lviUserActiveToggle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
