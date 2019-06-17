using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.Entertainment
{
    /// <summary>
    /// Interaction logic for Form_EntertainmentCreator.xaml
    /// </summary>
    public partial class Form_EntertainmentCreator : Window
    {
        private EntertainmentViewModel _evm;
        private Bridge _bridge;
        public Form_EntertainmentCreator()
        {
            InitializeComponent();

            _evm = DataContext as EntertainmentViewModel;
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            await _evm.Initialize(_bridge);

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
                _bridge.ShowErrorMessages();
            }
        }
    }
}
