using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HueLib;
using HueLib_base;
using WinHue3.Resources;
using Group = HueLib_base.Group;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for GroupCreator.xaml
    /// </summary>
    public partial class Form_GroupCreator : Window
    {
        /// <summary>
        /// Current bridge.
        /// </summary>
        private Bridge _bridge;

        /// <summary>
        /// Id of the modified or created group.
        /// </summary>
        private string _id;

        private GroupCreatorView gcv;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">Actual Bridge.</param>
        public Form_GroupCreator(Bridge bridge)
        {
            InitializeComponent();
            _bridge = bridge;
            gcv = new GroupCreatorView(HueObjectHelper.GetBridgeLights(_bridge));
            DataContext = gcv;
            
        }

        public Form_GroupCreator(Bridge bridge, HueObject selectedGroup)
        {
            InitializeComponent();
            _bridge = bridge;
            gcv = new GroupCreatorView(HueObjectHelper.GetBridgeLights(_bridge),(Group)selectedGroup);
            DataContext = gcv;
            Title = string.Format(GUI.GroupCreatorForm_ModifyingGroup_Title, ((Group)selectedGroup).name);
            btnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
            DataContext = gcv;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            Group grp = gcv.GetGroup();
            string result = "0";

            if (grp.Id == null)
            {
                string @group = _bridge.CreateGroup(grp);
                if (@group != null)
                    result = @group;
            }
            else
            {

                _bridge.ChangeGroup(grp);
                if (_bridge.lastMessages.SuccessCount >= 1)
                    result = grp.Id;
            }

            if (result != "0")
            {
                DialogResult = true;
                _id = result;
                Close();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);   
            }
        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

 

    }
}
