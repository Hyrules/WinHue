using System;
using System.Collections.Generic;
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
using HueLib;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_HotKeyCreator.xaml
    /// </summary>
    public partial class Form_HotKeyCreator : Window
    {
        private HotKeyCreatorView hkv;

        public Form_HotKeyCreator(Bridge br)
        {
            InitializeComponent();
            hkv = new HotKeyCreatorView(br);
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
    }
}
