using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Models;
using WinHue3.Philips_Hue.HueObjects.SensorObject;
using WinHue3.Resources;
using WinHue3.Utils;

namespace WinHue3.ViewModels
{
    public class SensorCreatorViewModel : ValidatableBindableBase
    {
        private SensorCreatorModel _sensorModel;
        private bool _isEditing;

        public SensorCreatorViewModel()
        {
            _sensorModel = new SensorCreatorModel();
        }

        public ISensor Sensor
        {
            get
            {
                ISensor sensor = SensorFactory.CreateSensor(SensorModel.Type);
                sensor.name = SensorModel.Name;
                sensor.manufacturername = SensorModel.Mfgname;
                sensor.modelid = SensorModel.ModelId;
                sensor.swversion = SensorModel.Swversion;
                sensor.uniqueid = SensorModel.Uniqueid;
                sensor.type = SensorModel.Type;
                sensor.SetConfig(SensorModel.Config);     
                return sensor;
            }
            set
            {
                IsEditing = true;
                SensorModel.Type = value.type;
                SensorModel.Name = value.name;
                SensorModel.Mfgname = value.manufacturername;
                SensorModel.ModelId = value.modelid;
                SensorModel.Swversion = value.swversion;
                SensorModel.Uniqueid = value.uniqueid;
                SensorModel.Config = value.GetConfig(); 
              
            }
        }

        public string CreateBtnText => IsEditing ? GUI.SensorCreatorForm_EditButton : GUI.SensorCreatorForm_CreateButton;
        public ICommand ChangeSensorTypeCommand => new RelayCommand(param => ChangeSensorType());

        private void ChangeSensorType()
        {
            SensorModel.Config = HueSensorConfigFactory.CreateSensorConfigFromSensorType(SensorModel.Type);
        }


        public SensorCreatorModel SensorModel
        {
            get => _sensorModel;
            set => SetProperty(ref _sensorModel,value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set { SetProperty(ref _isEditing,value); RaisePropertyChanged("NotEditing"); }
        }

        public bool NotEditing => !_isEditing;
    }
}
