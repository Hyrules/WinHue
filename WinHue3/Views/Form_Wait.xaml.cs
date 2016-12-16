using System;
using System.Windows;

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
