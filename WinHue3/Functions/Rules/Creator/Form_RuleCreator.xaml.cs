
using System.Threading.Tasks;
using System.Windows;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Resources;
using Bridge = WinHue3.Philips_Hue.BridgeObject.Bridge;

namespace WinHue3.Functions.Rules.Creator
{
    /// <summary>
    /// Interaction logic for Form_RuleCreator.xaml
    /// </summary>
    public partial class Form_RuleCreator : Window
    {
        private RuleCreatorViewModel _rcvm;
        private Bridge _bridge;
        private string _id;

        public Form_RuleCreator(Bridge bridge)
        {
            InitializeComponent();
            _rcvm = DataContext as RuleCreatorViewModel;
            _bridge = bridge;
            _id = null;
            Title = GUI.RuleCreatorForm_Create;
        }

        public Form_RuleCreator(Bridge bridge, Rule modifiedrule)
        {
            InitializeComponent();
            _rcvm = DataContext as RuleCreatorViewModel;
            _bridge = bridge;            
            _rcvm.Rule = modifiedrule;
            _id = modifiedrule.Id;
            Title = $"{GUI.RuleCreatorForm_Editing} {modifiedrule.name}";
        }

        public async Task Initialize()
        {
            await _rcvm.Initialize(_bridge);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool result;
            if (_id != null)
            {
                Rule _newrule = _rcvm.Rule;
                _newrule.Id = _id;
                result =_bridge.ModifyObject(_newrule);

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
