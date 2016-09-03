using System.Collections.Generic;
using System.Windows;
using HueLib2;
using WinHue3.Resources;
using Group = HueLib2.Group;

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
            HelperResult hr = HueObjectHelper.GetBridgeLights(_bridge);

            if (hr.Success)
            {
                gcv = new GroupCreatorView((List<HueObject>)hr.Hrobject);
                DataContext = gcv;
            }
            else
            {           
                MessageBoxError.ShowLastErrorMessages(_bridge);
                this.Close();
            }
        }

        public Form_GroupCreator(Bridge bridge, HueObject selectedGroup)
        {
            InitializeComponent();
            _bridge = bridge;
            HelperResult hr = HueObjectHelper.GetBridgeLights(_bridge);
            if (hr.Success)
            {
                gcv = new GroupCreatorView((List<HueObject>)hr.Hrobject, (Group)selectedGroup);
                Title = string.Format(GUI.GroupCreatorForm_ModifyingGroup_Title, ((Group)selectedGroup).name);
                btnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
                DataContext = gcv;
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
                this.Close();
            }
           
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            Group grp = gcv.GetGroup();

            if (grp.Id == null)
            {
                CommandResult bresult = _bridge.CreateObject<Group>(grp);
                if (bresult.Success)
                {
                    DialogResult = true;
                    _id = ((MessageCollection)bresult.resultobject)[0].ToString();
                    Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);                   
                }
                
            }
            else
            {

                CommandResult bresult = _bridge.ModifyObject<Group>(new Group() {name = grp.name,lights = grp.lights,@class = grp.@class},grp.Id);
                if (bresult.Success)
                {
                    DialogResult = true;
                    _id = grp.Id;
                    Close();

                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
            }

        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

 

    }
}
