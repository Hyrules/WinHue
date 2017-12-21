using MahApps.Metro.Controls;
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.User_Management
{
    /// <summary>
    /// Interaction logic for Form_ManageUsers.xaml
    /// </summary>
    public partial class Form_ManageUsers : MetroWindow
    {
        private ManageUsersViewModel _muvm;

        public Form_ManageUsers()
        {
            InitializeComponent();
            _muvm = this.DataContext as ManageUsersViewModel;
           
        }

        public async Task Initialize(Bridge bridge)
        {
            await _muvm.Initialize(bridge);
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
