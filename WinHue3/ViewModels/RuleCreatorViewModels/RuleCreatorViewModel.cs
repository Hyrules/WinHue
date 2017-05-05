using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using HueLib2;
using Action = HueLib2.Action;
using MessageBox = System.Windows.MessageBox;
using System.ComponentModel;
using WinHue3.Models;
using WinHue3.Utils;

namespace WinHue3.ViewModels
{
    public class RuleCreatorViewModelOld : ValidatableBindableBase
    {
        private Rule _rule;

        private RuleCreatorModel _ruleCreatorModel;
        private RuleConditionViewModel _ruleConditionViewModel;
        private RuleActionViewModel _ruleActionViewModel;

        #region CTOR

        public RuleCreatorViewModelOld()
        {
            _ruleCreatorModel = new RuleCreatorModel();
            _ruleConditionViewModel = new RuleConditionViewModel();
            _ruleActionViewModel = new RuleActionViewModel();
        }

        public void Initialize(List<HueObject> listObjects)
        {
            
            _rule = new Rule();
            RuleActionViewModel.ListDataStore = listObjects;
            RuleActionViewModel.ListDataStore.Add(new Sensor()
            {
                Id = "config",
                name = "config",
                state = new TimeSensorState()
            });
            RuleConditionViewModel.ListSensors.AddRange(listObjects.OfType<Light>().ToList());
            RuleConditionViewModel.ListSensors.AddRange(listObjects.OfType<Group>().ToList());
            RuleConditionViewModel.ListSensors.AddRange(listObjects.OfType<Sensor>().ToList());
        }

        public void Initialize(List<HueObject> listObjects, Rule modifiedRule)
        {
            _rule = modifiedRule;
            RuleActionViewModel.ListDataStore = listObjects;
            RuleActionViewModel.ListDataStore.Add(new Sensor()
            {
                Id = "config",
                name = "config",
                state = new TimeSensorState()            
            });

            RuleCreatorModel.Name = _rule.name;
            RuleCreatorModel.RuleEnabled = _rule.status;
            RuleConditionViewModel.ListSensors = listObjects.ToList();
            RuleConditionViewModel.ListConditions = new ObservableCollection<RuleCondition>(_rule.conditions);
            RuleActionViewModel.ListActions = new ObservableCollection<RuleAction>(_rule.actions);
            
        }

        #endregion

        #region PROPERTIES


        public RuleCreatorModel RuleCreatorModel
        {
            get { return _ruleCreatorModel; }
            set { SetProperty(ref _ruleCreatorModel, value); }
        }

        public RuleConditionViewModel RuleConditionViewModel
        {
            get { return _ruleConditionViewModel; }
            set { SetProperty(ref _ruleConditionViewModel,value); }
        }
      
        public RuleActionViewModel RuleActionViewModel
        {
            get { return _ruleActionViewModel; }
            set { SetProperty(ref _ruleActionViewModel,value); }
        }

        #endregion

        public Rule GetRule()
        {
            _rule.name = RuleCreatorModel.Name;
            _rule.status = RuleCreatorModel.RuleEnabled;
            _rule.conditions = new List<RuleCondition>(RuleConditionViewModel.ListConditions);
            _rule.actions = new List<RuleAction>(RuleActionViewModel.ListActions);
            return _rule;
        }

    }
}
