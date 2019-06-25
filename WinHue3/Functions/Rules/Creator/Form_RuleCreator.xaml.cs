using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.BridgeObject;
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
        private Bridge _bridge;

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

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            await _rcvm.Initialize(_bridge);
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
                result = _bridge.ModifyObject(_newrule);

            }
            else
            {
                result = _bridge.CreateObject(_rcvm.Rule);
                if (result)
                {
                    _id = _bridge.LastCommandMessages.LastSuccess.value;
                }
            }

            if (result)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                _bridge.ShowErrorMessages();
            }
        }

        public string GetCreatedOrModifiedID()
        {
            return _id;
        }

    }
}
