using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HueLib2;
using WinHue3.Resources;
using WinHue3.ViewModels;

namespace WinHue3
{

    
    /// <summary>
    /// Interaction logic for Form_RulesCreator2.xaml
    /// </summary>
    public partial class Form_RulesCreator2 : Window
    {
        private readonly Bridge _br;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
//        private Dictionary<PropertyInfo,dynamic> listproperties = new Dictionary<PropertyInfo, dynamic>();
        private Rule _editedRule;
        private RuleCreatorViewModelOld _rcv;
        private string id;

        public Form_RulesCreator2(Bridge bridge)
        {
            InitializeComponent();
            _br = bridge;
            _rcv = DataContext as RuleCreatorViewModelOld;

            HelperResult hr = HueObjectHelper.GetBridgeDataStore(bridge);
            if (hr.Success)
            {

                _rcv.Initialize((List<HueObject>)hr.Hrobject);
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(bridge);
            }

        }

        public Form_RulesCreator2(Bridge bridge, Rule rule)
        {
            InitializeComponent();
            _br = bridge;
            _rcv = DataContext as RuleCreatorViewModelOld;

            HelperResult hr = HueObjectHelper.GetBridgeDataStore(bridge);

            if (hr.Success)
            {
                _rcv.Initialize((List<HueObject>)hr.Hrobject, rule);

            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(bridge);
            }

            _editedRule = (Rule)rule;
            id = _editedRule.Id;
            Title = $"{GUI.RuleCreatorForm_Editing} {((Rule)rule).name}...";
            btnCreateRule.Content = GUI.RuleCreatorForm_Update;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCreateRule_Click(object sender, RoutedEventArgs e)
        {
            Rule newRule = _rcv.GetRule();
            CommandResult comres;

            if (_editedRule == null)
            {
                comres = _br.CreateObject<Rule>(newRule);
            }
            else
            {
                comres = _br.ModifyObject<Rule>(newRule, _editedRule.Id);
            }
     

            if (comres.Success)
            {
                log.Info(_editedRule == null ? $"Created new rule : {newRule.name}" : $"Updated rule : {newRule.name}");
                if (((MessageCollection) comres.resultobject)[0] is CreationSuccess)
                {
                    id = ((CreationSuccess)((MessageCollection)comres.resultobject)[0]).id;
                }
                else
                {
                    id = ((Success)((MessageCollection)comres.resultobject)[0]).id;
                }
               
                DialogResult = true;
                Close();
            }
            else
            {
                _br.ShowErrorMessages();
            }
                        
        }

        public string GetCreatedOrModifiedId()
        {
            return id ;
        }

        private void lbConditions_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_rcv.RuleConditionViewModel.SelectedCondition == null) e.Handled = true;
        }

        private void lbProperties_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_rcv.RuleActionViewModel.SelectedProperty.Equals(default(KeyValuePair<PropertyInfo,dynamic>))) e.Handled = true;
        }

        private void lbActions_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_rcv.RuleActionViewModel.SelectedAction == null) e.Handled = true;
        }
    }
}
