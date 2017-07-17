using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using HueLib2;
using HueLib2.Objects.Rules;
using WinHue3.Utils;
using WinHue3.Views;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using Rule = HueLib2.Rule;

namespace WinHue3.ViewModels
{
    public class RuleCreatorViewModel : ValidatableBindableBase
    {
        private DataStore _ds;
        private ObservableCollection<RuleConditionViewModel> _listConditions;
        private ObservableCollection<RuleActionViewModel> _listActions;
        private RuleConditionViewModel _ruleConditionViewModel;

        private ValidatableBindableBase _currentViewModel;

        public RuleCreatorViewModel()
        {

            _listActions = new ObservableCollection<RuleActionViewModel>();
            _listConditions = new ObservableCollection<RuleConditionViewModel>();
            _currentViewModel = new RuleConditionViewModel();
        }

        public void Initialize(DataStore ds)
        {
            _ds = ds;
        }

        public ObservableCollection<RuleConditionViewModel> ListConditions
        {
            get { return _listConditions; }
            set { SetProperty(ref _listConditions, value); }
        }

        public ObservableCollection<RuleActionViewModel> ListActions
        {
            get { return _listActions; }
            set { SetProperty(ref _listActions, value); }
        }

        private bool CanAddAction()
        {
            
            return true;
        }

        private void AddAction()
        {
      
        }

        private bool CanAddCondition()
        {
            return ListConditions.Count != 8;
        }

        private void AddCondition()
        {
            Form_RuleConditionCreator rcc = new Form_RuleConditionCreator(_ds) {Owner = Application.Current.MainWindow};          
            if (rcc.ShowDialog() == true)
            {
                
            }

        }

        private void RemoveCondition()
        {
            
        }

        public ICommand AddActionCommand => new RelayCommand(param => AddAction(),(param)=> CanAddAction());
        public ICommand AddConditionCommand => new RelayCommand(param => AddCondition(), (param)=> CanAddCondition());
        public ICommand RemoveConditionCommand => new RelayCommand(param => RemoveCondition(), (param) => CanRemoveCondition() );

        public RuleConditionViewModel SelectedCondition
        {
            get { return _ruleConditionViewModel; }
            set { SetProperty(ref _ruleConditionViewModel,value); }
        }

        public ValidatableBindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { _currentViewModel = value; }
        }

        private bool CanRemoveCondition()
        {
            return SelectedCondition != null;
        }

        private static void CastToType(Type type, string text, out object result)
        {
            try
            {
                if (type == typeof(byte))
                {
                    result = byte.Parse(text);
                    return;
                }

                if (type == typeof(int))
                {
                    result = int.Parse(text);
                    return;
                }

                if (type == typeof(ushort))
                {
                    result = ushort.Parse(text);
                    return;
                }

                if (type == typeof(bool))
                {
                    result = bool.Parse(text);
                    return;
                }

                if (type == typeof(float))
                {
                    result = float.Parse(text);
                    return;
                }

                if (type == typeof(string))
                {
                    if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
                    {
                        result = text;
                        return;
                    }
                }

                if (type == typeof(uint))
                {
                    result = uint.Parse(text);
                    return;

                }

                if (type == typeof(XY))
                {
                    result = XY.Parse(text);
                    return;
                }

                if (type == typeof(short))
                {
                    result = short.Parse(text);
                    return;
                }
                result = null;
            }
            catch (FormatException)
            {
                result = null;
                System.Windows.MessageBox.Show(GlobalStrings.Rule_NotProperFormat + type, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OverflowException)
            {
                result = null;
                System.Windows.MessageBox.Show(GlobalStrings.Rule_ValueNotInRange + type, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
