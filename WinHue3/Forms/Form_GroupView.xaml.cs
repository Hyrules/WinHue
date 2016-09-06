using System.Collections.Generic;
using System.Windows;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_GroupView : Window
    {
        private GroupViewView _gvv;
        public Form_GroupView()
        {
            InitializeComponent();

            CommandResult comlgt = BridgeStore.SelectedBridge.GetListObjects<Light>();
            CommandResult comgrp = BridgeStore.SelectedBridge.GetListObjects<Group>();

            if (comlgt.Success && comgrp.Success)
            {
                _gvv = new GroupViewView((Dictionary<string, Group>) comgrp.resultobject,
                    (Dictionary<string, Light>) comlgt.resultobject);
                DataContext = _gvv;
            }
            else
            {
                BridgeStore.SelectedBridge.ShowErrorMessages();
            }
        }
    }
}
