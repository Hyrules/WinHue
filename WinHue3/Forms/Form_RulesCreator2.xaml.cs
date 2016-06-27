using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HueLib;
using HueLib_base;
using WinHue3.Classes;
using Action = HueLib_base.Action;
using Group = HueLib_base.Group;

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
        private string _id;
        private readonly RuleCreatorView rcv;

        public Form_RulesCreator2(Bridge bridge)
        {
            InitializeComponent();
            _br = bridge;
            rcv = new RuleCreatorView(HueObjectHelper.GetBridgeDataStore(bridge));
            DataContext = rcv;
        }

        public Form_RulesCreator2(Bridge bridge, HueObject rule)
        {
            InitializeComponent();
            _br = bridge;
            rcv = new RuleCreatorView(HueObjectHelper.GetBridgeDataStore(bridge),rule);
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
            string cr = null;

            cr = editedRule == null ? _br.CreateRule(newRule) : _br.ModifyRule(editedRule.Id, newRule);

            if (cr != null)
            {
                log.Info(editedRule == null ? $"Created new rule : {newRule.name}" : $"Updated rule : {newRule.name}");
                DialogResult = true;
                _id = cr;
                Close();
            }
            else
            {
                _br.ShowErrorMessages();
            }
            
        }

        public string GetCreatedOrModifiedId()
        {
            return _id;
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
