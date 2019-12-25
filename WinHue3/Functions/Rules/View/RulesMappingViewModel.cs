using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Rules.View
{
    public class RulesMappingViewModel : ValidatableBindableBase
    {
        private Bridge _bridge;
        private ObservableCollection<Scene> _scenes;

        public RulesMappingViewModel()
        {

        }

        public ObservableCollection<Scene> Scenes { get => _scenes; set => SetProperty(ref _scenes, value); }

        public void Initialize(Bridge bridge, ObservableCollection<Scene> scenes)
        {
            _bridge = bridge;
            Scenes = scenes;
        }

    }
}
