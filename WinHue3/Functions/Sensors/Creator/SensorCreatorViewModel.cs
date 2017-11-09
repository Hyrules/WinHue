using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Models;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
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

        public Sensor Sensor
        {
            get
            {
                Sensor sensor = new Sensor
                {
                    name = SensorModel.Name,
                    manufacturername = SensorModel.Mfgname,
                    modelid = SensorModel.ModelId,
                    swversion = SensorModel.Swversion,
                    uniqueid = SensorModel.Uniqueid,
                    type = SensorModel.Type,
                    config = SensorModel.Config
                };

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
                SensorModel.Config = value.config; 
              
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
