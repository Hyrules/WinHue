using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.ComponentModel;

using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.BridgeFinder
{
    /// <summary>
    /// Interaction logic for Form_BridgeFinder.xaml
    /// </summary>
    public partial class Form_BridgeFinder : Window
    {
        private BridgeFinderViewModel _bfvm;
        private Bridge _br;
        public IPAddress newip { get; private set; }
        public bool cancelled { get; private set; }

        public Form_BridgeFinder(Bridge br)
        {
            InitializeComponent();
            _bfvm = this.DataContext as BridgeFinderViewModel;
            _br = br;
            BridgeFinder.ScanCompleted += BridgeFinder_ScanCompleted;
        }

        private void BridgeFinder_ScanCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
                if (e.Result != null) newip = (IPAddress) e.Result;
            Close();
        }

        public bool IpFound()
        {
            return _bfvm.Found;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            _bfvm.FindBridge(_br);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancelled = true;
            _bfvm.CancelSearch();
        }
    }
}
