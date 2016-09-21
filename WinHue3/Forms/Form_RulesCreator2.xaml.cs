using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HueLib2;

namespace WinHue3
{

    
    /// <summary>
    /// Interaction logic for Form_RulesCreator2.xaml
    /// </summary>
    public partial class Form_RulesCreator2 : Window
    {
        private readonly Bridge _br;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<PropertyInfo,dynamic> listproperties = new Dictionary<PropertyInfo, dynamic>();
        private readonly Rule editedRule;
        private readonly RuleCreatorView rcv;

        public Form_RulesCreator2(Bridge bridge)
        {
            InitializeComponent();
            _br = bridge;
            HelperResult hr = HueObjectHelper.GetBridgeDataStore(bridge);
            List<HueObject> listobj;
            if (hr.Success)
            {
                listobj = (List<HueObject>) hr.Hrobject;
            }
            else
            {
                listobj = new List<HueObject>();
            }
            rcv = new RuleCreatorView(listobj);
            DataContext = rcv;
        }

        public Form_RulesCreator2(Bridge bridge, HueObject rule)
        {
            InitializeComponent();
            _br = bridge;
            HelperResult hr = HueObjectHelper.GetBridgeDataStore(bridge);
            List<HueObject> listobj;
            if (hr.Success)
            {
                listobj = (List<HueObject>)hr.Hrobject;
            }
            else
            {
                listobj = new List<HueObject>();
            }
            rcv = new RuleCreatorView(listobj,rule);
            DataContext = rcv;
            editedRule = (Rule)rule;
            Title = $"Editing rule {((Rule)rule).name}...";
            btnCreateRule.Content = "Update";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCreateRule_Click(object sender, RoutedEventArgs e)
        {
            Rule newRule = rcv.GetRule();
            newRule.owner = null;
            newRule.created = null;
            newRule.timestriggered = null;
            newRule.lasttriggered = null;

            CommandResult comres = editedRule == null ? _br.CreateObject<Rule>(newRule) : _br.ModifyObject<Rule>(newRule, editedRule.Id);

            if (comres.Success)
            {
                log.Info(editedRule == null ? $"Created new rule : {newRule.name}" : $"Updated rule : {newRule.name}");
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
            return editedRule.Id;
        }

        private void lbConditions_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (rcv.SelectedCondition == null) e.Handled = true;
        }

        private void lbProperties_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (rcv.SelectedProperty.Equals(default(KeyValuePair<PropertyInfo,dynamic>))) e.Handled = true;
        }

        private void lbActions_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (rcv.SelectedAction == null) e.Handled = true;
        }
    }
}
