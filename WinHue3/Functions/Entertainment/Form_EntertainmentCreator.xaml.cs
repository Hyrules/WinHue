using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.BridgeManager;

namespace WinHue3.Functions.Entertainment
{
    /// <summary>
    /// Interaction logic for Form_EntertainmentCreator.xaml
    /// </summary>
    public partial class Form_EntertainmentCreator : Window
    {
        private EntertainmentViewModel _evm;

        public Form_EntertainmentCreator()
        {
            InitializeComponent();
            _evm = DataContext as EntertainmentViewModel;
        }

        public async Task Initialize()
        {
            await _evm.Initialize();

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_evm.SaveEntertainment())
            {
                Close();
            }
            else
            {
                BridgesManager.Instance.SelectedBridge.ShowErrorMessages();
            }
        }
    }
}
