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
        private readonly Bridge _bridge;
        public Form_GroupView(Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();

            CommandResult comlgt = _bridge.GetListObjects<Light>();
            if (!comlgt.Success) return;
            CommandResult comgrp = _bridge.GetListObjects<Group>();
            if (!comgrp.Success) return;

            _gvv = new GroupViewView((Dictionary<string, Group>) comgrp.resultobject,(Dictionary<string, Light>) comlgt.resultobject);
            DataContext = _gvv;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(_gvv == null)
                Close();
        }
    }
}
