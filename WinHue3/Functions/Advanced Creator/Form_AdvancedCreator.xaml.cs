using System;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.Advanced_Creator
{
    /// <summary>
    /// Interaction logic for Form_AdvancedCreator.xaml
    /// </summary>
    public partial class Form_AdvancedCreator : Window
    {
        private AdvancedCreatorViewModel _acvm;

        public Form_AdvancedCreator(Bridge bridge)
        {
            InitializeComponent();
            _acvm = DataContext as AdvancedCreatorViewModel;
            _acvm.Initialize(bridge);
            _acvm.OnObjectCreated += _acvm_OnObjectCreated;
        }

        private void _acvm_OnObjectCreated(object sender, EventArgs e)
        {
            OnObjectCreated?.Invoke(this,EventArgs.Empty);
        }

        public event EventHandler OnObjectCreated;

        private void RibbonApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
