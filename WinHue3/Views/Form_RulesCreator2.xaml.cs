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
//        private Dictionary<PropertyInfo,dynamic> listproperties = new Dictionary<PropertyInfo, dynamic>();
        private Rule editedRule;
        private RuleCreatorView rcv;
        private string id;

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
            id = editedRule.Id;
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
            CommandResult comres;

            if (editedRule == null)
            {
                comres = _br.CreateObject<Rule>(newRule);
            }
            else
            {
                comres = _br.ModifyObject<Rule>(newRule, editedRule.Id);
            }
     

            if (comres.Success)
            {
                log.Info(editedRule == null ? $"Created new rule : {newRule.name}" : $"Updated rule : {newRule.name}");
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
