using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.HotKeys.Creator
{
    /// <summary>
    /// Interaction logic for Form_HotKeyCreator.xaml
    /// </summary>
    public partial class Form_HotKeyCreator : Window
    {
        private HotKeyCreatorViewModel _hkv;
        private Bridge _bridge;
        public Form_HotKeyCreator()
        {
            InitializeComponent();
            _hkv = DataContext as HotKeyCreatorViewModel;           
        }

        public async Task Initialize(Bridge bridge, ObservableCollection<HotKey> listhotkeys)
        {
            _bridge = bridge;
            await _hkv.Initialize(_bridge,listhotkeys);
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            _hkv.SaveHotKeys();
            Close();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(_hkv.CanRecordKeyUp)
                _hkv.CaptureHotkey(e);
        }

        private void btnHelpGeneric_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GlobalStrings.Form_Hotkey_Help_Generic, GlobalStrings.Help, MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}
