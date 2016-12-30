using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;
using WinHue3.Models;

namespace WinHue3.ViewModels
{
    public class ResourceLinkCreatorViewModel : ValidatableBindableBase
    {
        private ResourceLinkCreatorModel _resourceLinkCreatorModel;
        private ObservableCollection<HueObject> _listHueObjects;
        private ObservableCollection<HueObject> _selectedLinkObjects;


        public ResourceLinkCreatorViewModel()
        {
            _resourceLinkCreatorModel = new ResourceLinkCreatorModel();
            _listHueObjects = new ObservableCollection<HueObject>();
            _selectedLinkObjects = new ObservableCollection<HueObject>();
        }

        public Resourcelink Resourcelink
        {
            get
            {
                Resourcelink rl = new Resourcelink {name = _resourceLinkCreatorModel.Name, links = new List<string>(),description = _resourceLinkCreatorModel.Description};
                foreach (HueObject obj in _selectedLinkObjects)
                {
                    string typename = string.Empty;
                    if (obj.GetType().BaseType == typeof(HueObject))
                    {
                        typename = obj.GetType().ToString().Replace(obj.GetType().Namespace, "").Replace(".", "").ToLower() + "s";
                    }
                    else
                    {
                        typename = obj.GetType().BaseType.ToString().Replace(obj.GetType().Namespace, "").Replace(".", "").ToLower() + "s";
                    }
                    rl.links.Add($"/{typename}/{obj.Id}");
                }
                return rl;
            }
            set
            {
                
            }
        }

        public ResourceLinkCreatorModel LinkCreatorModel
        {
            get { return _resourceLinkCreatorModel; }
            set { SetProperty(ref _resourceLinkCreatorModel,value); }
        }

        public ObservableCollection<HueObject> ListHueObjects
        {
            get { return _listHueObjects; }
            set { SetProperty(ref _listHueObjects, value); }
        }

        public ObservableCollection<HueObject> SelectedLinkObjects
        {
            get { return _selectedLinkObjects; }
            set { SetProperty(ref _selectedLinkObjects,value); }
        }

    }
}
