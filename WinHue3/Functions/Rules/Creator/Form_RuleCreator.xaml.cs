
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Functions.BridgeManager;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Resources;

namespace WinHue3.Functions.Rules.Creator
{
    /// <summary>
    /// Interaction logic for Form_RuleCreator.xaml
    /// </summary>
    public partial class Form_RuleCreator : Window
    {
        private RuleCreatorViewModel _rcvm;
        private string _id;

        public Form_RuleCreator()
        {
            InitializeComponent();
            _rcvm = DataContext as RuleCreatorViewModel;
            _id = null;
            Title = GUI.RuleCreatorForm_Create;
        }

        public Form_RuleCreator(Rule modifiedrule)
        {
            InitializeComponent();
            _rcvm = DataContext as RuleCreatorViewModel;
        
            _rcvm.Rule = modifiedrule;
            _id = modifiedrule.Id;
            Title = $"{GUI.RuleCreatorForm_Editing} {modifiedrule.name}";
        }

        public async Task Initialize()
        {
            await _rcvm.Initialize();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool result;
            if (_id != null)
            {
                Rule _newrule = _rcvm.Rule;
                _newrule.Id = _id;
                result = BridgesManager.Instance.SelectedBridge.ModifyObject(_newrule);

            }
            else
            {
                result = BridgesManager.Instance.SelectedBridge.CreateObject(_rcvm.Rule);
                if (result)
                {
                    _id = BridgesManager.Instance.SelectedBridge.LastCommandMessages.LastSuccess.value;
                }
            }

            if (result)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                BridgesManager.Instance.SelectedBridge.ShowErrorMessages();
            }
        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

    }
}
