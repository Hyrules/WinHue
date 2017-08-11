using System;
using System.Windows;
using WinHue3.ViewModels;

namespace WinHue3.Views
{
    /// <summary>
    /// Interaction logic for Form_Wait.xaml
    /// </summary>
    public partial class Form_Wait : Window
    {
        private WaitViewModel wv;

        public Form_Wait()
        {
            InitializeComponent();
            
        }

        public void ShowWait(string message, TimeSpan duration, Window owner)
        {
            Owner = owner;
            wv = DataContext as WaitViewModel;
            wv.Message = message;
            wv.WaitTime = duration;
            wv.OnWaitComplete += Wv_OnWaitComplete;          
            wv.StartWait();
            this.ShowDialog();
        }

        private void Wv_OnWaitComplete(object sender, EventArgs e)
        {
            Close();
        }
    }
}
