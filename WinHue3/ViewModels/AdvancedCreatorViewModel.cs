using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;

namespace WinHue3.ViewModels
{
    public class AdvancedCreatorViewModel : ValidatableBindableBase
    {
        private string _text;
        private Bridge _bridge;
        private string _type;

        public AdvancedCreatorViewModel()
        {
            Text = string.Empty;
        }

        public void Initialize(Bridge bridge)
        {
            _bridge = bridge;
        }

        private void CreateScheduleTemplate()
        {
            Type = "schedule";
            Text = @"{
    ""name"" : ""{YOUR NAME}"",
    ""description"" : ""{YOUR DESCRIPTION}"",
    ""localtime"" : ""{YYYY-MM-DDTHH:mm:ss}"",
    ""command"" : {
        ""address"": ""/api/" + _bridge.ApiKey + @"/groups/1/action"",
        ""method"": ""PUT"",
        ""body"": {
            ""on"" : true
        }
    }
}";
        }

        private void CreateSensorTemplate()
        {
        }

        private void CreateRuleTemplate()
        {
        }

        private void CreateRessourceLinkTemplate()
        {
        }

        private void CreateGroupTemplate()
        {
        }


        public ICommand CreateScheduleTemplateCommand => new RelayCommand(param => CreateScheduleTemplate(), (param) => Type == string.Empty);
        public ICommand CreateSensorTemplateCommand => new RelayCommand(param => CreateSensorTemplate(), (param) => Type == string.Empty);
        public ICommand CreateRuleTemplateCommand => new RelayCommand(param => CreateRuleTemplate(), (param) => Type == string.Empty);
        public ICommand CreateRessourceLinkTemplateCommand => new RelayCommand(param => CreateRessourceLinkTemplate(), (param) => Type == string.Empty);
        public ICommand CreateGroupTemplateCommand => new RelayCommand(param => CreateGroupTemplate(), (param) => Type == string.Empty);

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text,value); }
        }

        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type,value); }
        }
    }
}