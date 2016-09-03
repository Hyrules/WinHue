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
        private Bridge _br;
        public Form_GroupView(Bridge br)
        {
            InitializeComponent();
            _br = br;
            CommandResult comlgt = _br.GetListObjects<Light>();
            CommandResult comgrp = _br.GetListObjects<Group>();

            if (comlgt.Success && comgrp.Success)
            {
                _gvv = new GroupViewView((Dictionary<string, Group>) comgrp.resultobject,
                    (Dictionary<string, Light>) comlgt.resultobject);
                DataContext = _gvv;
            }

        }
    }
}
