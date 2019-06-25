using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Newtonsoft.Json;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Utils;
using MessageBox = System.Windows.Forms.MessageBox;


namespace WinHue3.Functions.Advanced_Creator
{
    public class AdvancedCreatorViewModel : ValidatableBindableBase
    {
        private string _text;
        private string _type;
        private WebRequestType _requestType;
        private string _url;
        private Bridge _bridge;

        public AdvancedCreatorViewModel()
        {
            _text = string.Empty;
            _type = string.Empty;
            _url = string.Empty;
            _requestType = WebRequestType.Post;
        }

        public void Initialize(Bridge bridge)
        {
            _bridge = bridge;
        }

        private void CreateScheduleTemplate()
        {
            Type = "schedules";
            RequestType = WebRequestType.Post;
            Url = _bridge.BridgeUrl + "/schedules";
            Text = @"{
    ""name"" : ""{YOUR NAME}"",
    ""description"" : ""{YOUR DESCRIPTION}"",
    ""localtime"" : ""{YYYY-MM-DDTHH:mm:ss}"",
    ""command"" : {
        ""address"": ""/api/" + _bridge + @"/groups/1/action"",
        ""method"": ""PUT"",
        ""body"": {
            ""on"" : true
        }
    }
}";
        }

        private void CreateSensorTemplate()
        {
            Type = "sensors";
            RequestType = WebRequestType.Post;
            Url = _bridge.BridgeUrl + "/sensors";
            Text = @"{
    ""name"" : ""YOUR NAME"",
    ""description"" : ""YOUR DESCRIPTION"",
    ""modelid"": """".
    ""swversion"": """",
    ""type"": """",
    ""uniqueid"": """"
    ""manufacturername"": ""
    ""state"":{
        ...
    }
}";
        }

        private void CreateRuleTemplate()
        {
            Type = "rules";
            RequestType = WebRequestType.Post;
            Url = _bridge.BridgeUrl + "/rules";
            Text = @"{  
    ""name"":""YOUR NAME"",
    ""conditions"":[
        {
            ""address"":""/sensors/2/state/buttonevent"",
            ""operator"":""eq"",
            ""value"":""16""
        },
        {
            ...
        }
    ],
    ""actions"":[
        {
            ""address"":""/groups/0/action"",
            ""method"":""PUT"", 
            ""body"":{
                ""scene"":""S3""
            }
        },
        {
            ...
        }
    ]
}";
        }

        private void CreateResourceLinkTemplate()
        {
            Type = "resourcelinks";
            RequestType = WebRequestType.Post;
            Url = _bridge.BridgeUrl + "/resourcelinks";
            Text = @"{
    ""name"": ""Sunrise"",
    ""description"": ""Carla's wakeup experience"",
    ""type"":""Link"",
    ""class"": 1,
    ""owner"": ""78H56B12BAABCDEF"",
    ""links"": [""/schedules/2"", ""/schedules/3"",
              ""/scenes/ABCD"", ""/scenes/EFGH"", ""/groups/8""]
    }
";
        }

        private void CreateGroupTemplate()
        {
            Type = "groups";
            RequestType = WebRequestType.Post;
            Url = _bridge.BridgeUrl + "/groups";
            Text = @"{
    ""name"": ""Living room"",
    ""type"": ""Room"",
    ""class"": ""Living room"",
    ""lights"": [
        ""3"",
        ""4""
    ]
    }

";
        }

        private void ClearTemplate()
        {
            Type = string.Empty;
            Text = string.Empty;
            Url = string.Empty;
            RequestType = WebRequestType.Post;

        }

        private async Task Send()
        {
            DialogResult result = DialogResult.Yes;

            if (RequestType == WebRequestType.Delete)
            {
                result = MessageBox.Show(GlobalStrings.AdvancedCreator_WarningDelete, GlobalStrings.Warning,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }

            if (result == DialogResult.Yes)
            {
                string json = await _bridge.SendRawCommandAsyncTask(Url, Text, RequestType);
                if (json == null)
                {
                    _bridge.ShowErrorMessages();
                }
                else
                {
                    if (RequestType == WebRequestType.Get)
                    {
                        Text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
                    }
                    else
                    {
                        MessageBox.Show(_bridge.LastCommandMessages.ToString(), GlobalStrings.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        OnObjectCreated?.Invoke(this, EventArgs.Empty);
                    }
                
                
                }
            }
        }

        private void CreateSceneTemplate()
        {
            Type = "scenes";
            RequestType = WebRequestType.Post;
            Url = _bridge.BridgeUrl + "/scenes";
            Text = @"{
    ""lights""
        :[""3"",""2"",""5""],
    ""recycle"":true,
    ""name"":""My Scene"",
    ""appdata"":{
        ""data"":""My App Data"",
        ""version"":1
    },
    ""picture"":""ABC123DEF456"" 
}";
        }

        private void CreateSceneStateTemplate()
        {
            Type = "scenes";
            RequestType = WebRequestType.Put;
            Url = _bridge.BridgeUrl + "/scenes/<scene id>/lights/<light id>/state";
            Text = @"{ 
    ""on"":true,
    ""bri"":255,
    ""hue"":12345,
    ...

}";
        }

        public ICommand CreateScheduleTemplateCommand => new RelayCommand(param => CreateScheduleTemplate(), (param) => Type == string.Empty);
        public ICommand CreateSensorTemplateCommand => new RelayCommand(param => CreateSensorTemplate(), (param) => Type == string.Empty);
        public ICommand CreateRuleTemplateCommand => new RelayCommand(param => CreateRuleTemplate(), (param) => Type == string.Empty);
        public ICommand CreateResourceLinkTemplateCommand => new RelayCommand(param => CreateResourceLinkTemplate(), (param) => Type == string.Empty);
        public ICommand CreateGroupTemplateCommand => new RelayCommand(param => CreateGroupTemplate(), (param) => Type == string.Empty);
        public ICommand ClearTemplateCommand => new RelayCommand(param => ClearTemplate(), (param) => Text != string.Empty);
        public ICommand SendCommand => new AsyncRelayCommand(param => Send(), (param) => CanSendCommand());

        private bool CanSendCommand()
        {
            return !string.IsNullOrEmpty(Url) && !string.IsNullOrWhiteSpace(Url) && Uri.IsWellFormedUriString(Url, UriKind.Absolute);
        }

        public ICommand CreateSceneTemplateCommand => new RelayCommand(param => CreateSceneTemplate(), (param) => Text == string.Empty);
        public ICommand CreateSceneStateCommand => new RelayCommand(param => CreateSceneStateTemplate(), (param) => Text == string.Empty);
        public ICommand SetBridgeUrlCommand => new RelayCommand(param => SetBridgeUrl());
        
        private void SetBridgeUrl()
        {
            Url = _bridge.BridgeUrl;
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text,value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type,value);
        }

        public WebRequestType RequestType
        {
            get => _requestType;
            set => SetProperty(ref _requestType,value);
        }

        public string Url
        {
            get => _url;
            set => SetProperty(ref _url,value);
        }

        public event EventHandler OnObjectCreated;

    }
}