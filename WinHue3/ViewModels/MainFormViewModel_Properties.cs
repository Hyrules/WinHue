using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using HueLib2;

namespace WinHue3.ViewModels
{
    public partial class MainFormViewModel
    {
        private Form_EventLog _eventlogform;
        private ObservableCollection<Bridge> _listBridges;
        private HueObject _selectedObject;
        private Bridge _selectedBridge;
        private Supportedlight _selectedModel;

        public Bridge SelectedBridge
        {
            get { return _selectedBridge; }
            set
            {
                SetProperty(ref _selectedBridge,value);
                RefreshView();              
            }
        }

        public ObservableCollection<HueObject> ListBridgeObjects
        {
            get { return _listBridgeObjects; }
            set { SetProperty(ref _listBridgeObjects, value); }
        }

        public string Lastmessage
        {
            get { return _lastmessage; }
            set { SetProperty(ref _lastmessage,value); }
        }

        public ObservableCollection<Bridge> ListBridges
        {
            get { return _listBridges; }
            set { SetProperty(ref _listBridges,value); }
        }

        public HueObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                SetProperty(ref _selectedObject,value);
                SetMainFormModel();
            }
        }

        public Form_EventLog Eventlogform
        {
            get { return _eventlogform; }
            set { SetProperty(ref _eventlogform,value); }
        }

        public Supportedlight SelectedModel
        {
            get{return _selectedModel;}
            set{SetProperty(ref _selectedModel,value);}
        }
    }
}
