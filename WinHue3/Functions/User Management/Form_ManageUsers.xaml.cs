using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.ViewModels;

namespace WinHue3.Views
{
    /// <summary>
    /// Interaction logic for Form_ManageUsers.xaml
    /// </summary>
    public partial class Form_ManageUsers : Window
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
