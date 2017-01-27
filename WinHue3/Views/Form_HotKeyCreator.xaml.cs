using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HueLib2;
using WinHue3.Resources;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_HotKeyCreator.xaml
    /// </summary>
    public partial class Form_HotKeyCreator : Window
    {
        private HotKeyCreatorViewModel _hkv;

        public Form_HotKeyCreator(Bridge bridge)
        {
            InitializeComponent();
            _hkv = DataContext as HotKeyCreatorViewModel;
            _hkv.Initialize(bridge);   
               
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

        public List<HotKey> GetHotKeys()
        {
            return _hkv.ListHotKeys.ToList();
        }

        private void btnHelpGeneric_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GlobalStrings.Form_Hotkey_Help_Generic, GlobalStrings.Help, MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}
