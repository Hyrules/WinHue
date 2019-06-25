using System;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;

namespace WinHue3.Functions.Advanced_Creator
{
    /// <summary>
    /// Interaction logic for Form_AdvancedCreator.xaml
    /// </summary>
    public partial class Form_AdvancedCreator
    {
        private AdvancedCreatorViewModel _acvm;

        public Form_AdvancedCreator()
        {
            InitializeComponent();
            _acvm = DataContext as AdvancedCreatorViewModel;
            _acvm.OnObjectCreated += _acvm_OnObjectCreated;
        }

        public void Initialize(Bridge bridge)
        {
            _acvm.Initialize(bridge);
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
