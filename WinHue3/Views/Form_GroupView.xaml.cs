using System.Collections.Generic;
using System.Windows;
using HueLib2;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_GroupView : Window
    {
        private GroupViewViewModel _gvv;
        private readonly Bridge _bridge;
        public Form_GroupView(Bridge bridge)
        {
            _bridge = bridge;
            InitializeComponent();
            CommandResult comlgt = _bridge.GetListObjects<Light>();
            _gvv = DataContext as GroupViewViewModel;

            if (comlgt.Success)
            {
                CommandResult comgrp = _bridge.GetListObjects<Group>();
                if (comgrp.Success)
                {
                    
                    _gvv.Initialize((Dictionary<string, Group>)comgrp.resultobject, (Dictionary<string, Light>)comlgt.resultobject);
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(bridge);
                }

                DataContext = _gvv;

            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(bridge);
            }



        }

    }
}
