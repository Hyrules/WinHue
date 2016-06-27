using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_Wait.xaml
    /// </summary>
    public partial class Form_Wait : Window
    {
        private WaitView wv;

        public Form_Wait()
        {
            InitializeComponent();
            
        }

        public void ShowWait(string message, TimeSpan duration, Window owner)
        {
            Owner = owner;            
            wv = new WaitView(message, duration);
            wv.OnWaitComplete += Wv_OnWaitComplete;
            DataContext = wv;
            wv.StartWait();
            this.ShowDialog();
        }

        private void Wv_OnWaitComplete(object sender, EventArgs e)
        {
            Close();
        }
    }
}
