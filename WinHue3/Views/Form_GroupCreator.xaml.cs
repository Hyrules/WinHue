using System.Collections.Generic;
using System.Windows;
using HueLib2;
using ICSharpCode.NRefactory.Semantics;
using WinHue3.Resources;
using WinHue3.ViewModels;
using Group = HueLib2.Group;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for GroupCreator.xaml
    /// </summary>
    public partial class Form_GroupCreator : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Id of the modified or created group.
        /// </summary>
        private string _id;

        private GroupCreatorViewModel gcvm;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="bridge">Actual Bridge.</param>
        public Form_GroupCreator()
        {
            InitializeComponent();
            gcvm = this.DataContext as GroupCreatorViewModel;
        }

        public Form_GroupCreator(HueObject selectedGroup)
        {
            InitializeComponent();
            gcvm = this.DataContext as GroupCreatorViewModel;
            gcvm.Group = (Group) selectedGroup;
            btnCreateGroup.Content = GUI.GroupCreatorForm_ModifyGroupButton;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            if (gcvm.Group.Id == null)
            {
                CommandResult bresult = BridgeStore.SelectedBridge.CreateObject<Group>(gcvm.Group);
                if (bresult.Success)
                {
                    DialogResult = true;
                    log.Info(bresult.resultobject);
                    _id = ((MessageCollection)bresult.resultobject)[0].ToString();
                    Close();
                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);                   
                }
                
            }
            else
            {

                CommandResult bresult = BridgeStore.SelectedBridge.ModifyObject<Group>(gcvm.Group, gcvm.Group.Id);
                if (bresult.Success)
                {
                    DialogResult = true;
                    _id = gcvm.Group.Id;
                    Close();

                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
                }
            }

        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

    }
}
