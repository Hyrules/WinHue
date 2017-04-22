using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;
using WinHue3.Models;
using WinHue3.Resources;
using WinHue3.Validation;

namespace WinHue3.ViewModels
{
    public class ResourceLinkCreatorViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ResourceLinkCreatorModel _resourceLinkCreatorModel;
        private ObservableCollection<HueObject> _listHueObjects;
        private ObservableCollection<HueObject> _selectedLinkObjects;
        private bool _isEditing;
        private string _id;

        public ResourceLinkCreatorViewModel()
        {
            _resourceLinkCreatorModel = new ResourceLinkCreatorModel();
            _listHueObjects = new ObservableCollection<HueObject>();
            _selectedLinkObjects = new ObservableCollection<HueObject>();
            _id = null;
        }

        public Resourcelink Resourcelink
        {
            get
            {
                Resourcelink rl = new Resourcelink {name = LinkCreatorModel.Name, links = new List<string>(),description = LinkCreatorModel.Description,classid = LinkCreatorModel.ClassId, recycle = LinkCreatorModel.Recycle,Id = _id};
                foreach (HueObject obj in _selectedLinkObjects)
                {
                    string typename = string.Empty;
                    if (obj.GetType().BaseType == typeof(HueObject))
                    {
                        typename = obj.GetType().ToString().Replace(obj.GetType().Namespace, "").Replace(".", "").ToLower() + "s";
                    }
                    else
                    {
                        typename = obj.GetType().BaseType?.ToString().Replace(obj.GetType().Namespace, "").Replace(".", "").ToLower() + "s";
                    }
                    rl.links.Add($"/{typename}/{obj.Id}");
                }
                log.Info($"Getting Resource Link : {Serializer.SerializeToJson(rl)}");
                return rl;
            }
            set
            {
                IsEditing = true;
                Resourcelink rl = value;
                log.Info($"Setting Resource Link : {Serializer.SerializeToJson(rl)}");
                _id = rl.Id;
                LinkCreatorModel.Name = rl.name;
                LinkCreatorModel.ClassId = rl.classid;
                LinkCreatorModel.Description = rl.description;
                LinkCreatorModel.Recycle = rl.recycle;
                ObservableCollection<HueObject> listsel = new ObservableCollection<HueObject>();
                foreach (string s in rl.links)
                {
                    string[] parts = s.Split('/');
                    Assembly asm = typeof(Light).Assembly;
                    parts[1] = parts[1].TrimEnd('s');
                    parts[1] = parts[1].First().ToString().ToUpper() + string.Join("", parts[1].Skip(1));

                    Type objtype = Type.GetType($"HueLib2.{parts[1]}, {asm}");

                    HueObject obj = ListHueObjects.First(x => x.Id == parts[2] && x.GetType() == objtype);
                    if (obj != null)
                    {
                        listsel.Add(obj);
                        
                    }

                    
                }
                SelectedLinkObjects = listsel;
            }
        }

        public string BtnSaveText => IsEditing ? GUI.ResourceLinkCreatorForm_Editing : GUI.ResourceLinkCreatorForm_Create;

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

        [MinimumCount(1, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "ResourceLinks_SelectAtLeastOne")]
        public ObservableCollection<HueObject> SelectedLinkObjects
        {
            get { return _selectedLinkObjects; }
            set { SetProperty(ref _selectedLinkObjects,value); }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            set { SetProperty(ref _isEditing,value); RaisePropertyChanged("BtnSaveText"); }
        }

    }
}
