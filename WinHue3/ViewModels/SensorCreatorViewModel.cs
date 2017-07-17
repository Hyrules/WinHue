using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using WinHue3.Models;
using WinHue3.Resources;

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
                Sensor sensor = new Sensor()
                {
                    Name = SensorModel.Name,
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
                SensorModel.Name = value.Name;
                SensorModel.Mfgname = value.manufacturername;
                SensorModel.ModelId = value.modelid;
                SensorModel.Swversion = value.swversion;
                SensorModel.Uniqueid = value.uniqueid;
                SensorModel.Config = value.config;
            }
        }

        public string CreateBtnText => IsEditing ? GUI.SensorCreatorForm_EditButton : GUI.SensorCreatorForm_CreateButton;

        public SensorCreatorModel SensorModel
        {
            get { return _sensorModel; }
            set { SetProperty(ref _sensorModel,value); }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set { SetProperty(ref _isEditing,value); RaisePropertyChanged("NotEditing"); }
        }

        public bool NotEditing => !_isEditing;
    }
}
