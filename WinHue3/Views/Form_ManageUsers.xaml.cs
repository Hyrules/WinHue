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
using System.Windows.Shapes;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_ManageUsers.xaml
    /// </summary>
    public partial class Form_ManageUsers : Window
    {
        private ManageUsersViewModel _muvm;

        public Form_ManageUsers(Bridge bridge)
        {
            InitializeComponent();
            _muvm = this.DataContext as ManageUsersViewModel;
            _muvm.Bridge = bridge;

        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
