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
            List<Light> comlgt = HueObjectHelper.GetObjectsList<Light>(_bridge);
            _gvv = DataContext as GroupViewViewModel;

            if (comlgt != null)
            {
                List<Group> comgrp = HueObjectHelper.GetObjectsList<Group>(_bridge);
                if (comgrp != null)
                {
                    
                    _gvv.Initialize(comgrp, comlgt);
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
