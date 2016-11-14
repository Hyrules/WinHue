using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using HueLib2;
using WinHue3.Resources;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_HotKeyCreator.xaml
    /// </summary>
    public partial class Form_HotKeyCreator : Window
    {
        private HotKeyCreatorView hkv;

        public Form_HotKeyCreator()
        {
            InitializeComponent();
            hkv = new HotKeyCreatorView();
            DataContext = hkv;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            hkv.SaveHotKeys();
            Close();
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(hkv.canRecordKeyUp)
                hkv.CaptureHotkey(e);

        }

        public List<HotKey> GetHotKeys()
        {
            return hkv.ListHotkeys.ToList();
        }

        private void btnHelpGeneric_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GlobalStrings.Form_Hotkey_Help_Generic, GlobalStrings.Help, MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}
