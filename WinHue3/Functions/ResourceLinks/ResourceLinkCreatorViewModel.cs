using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using WinHue3.Philips_Hue.Communication;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Resources;
using WinHue3.Utils;
using WinHue3.Validations;

namespace WinHue3.Functions.ResourceLinks
{
    public class ResourceLinkCreatorViewModel : ValidatableBindableBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ResourceLinkCreatorModel _resourceLinkCreatorModel;
        private ObservableCollection<IHueObject> _listIHueObjects;
        private ObservableCollection<IHueObject> _selectedLinkObjects;
        private bool _isEditing;
        private string _id;

        public ResourceLinkCreatorViewModel()
        {
            _resourceLinkCreatorModel = new ResourceLinkCreatorModel();
            _listIHueObjects = new ObservableCollection<IHueObject>();
            _selectedLinkObjects = new ObservableCollection<IHueObject>();
            _id = null;
        }

        public Resourcelink Resourcelink
        {
            get
            {
                Resourcelink rl = new Resourcelink {name = LinkCreatorModel.Name, links = new StringCollection(),description = LinkCreatorModel.Description,classid = LinkCreatorModel.ClassId, recycle = LinkCreatorModel.Recycle,Id = _id};
                foreach (IHueObject obj in _selectedLinkObjects)
                {
                    string typename = obj.GetType().Name.ToLower() + "s";
                    rl.links.Add($"/{typename}/{obj.Id}");
                }
                log.Info($"Getting Resource Link : {Serializer.SerializeJsonObject(rl)}");
                return rl;
            }
            set
            {
                IsEditing = true;
                Resourcelink rl = value;
                log.Info($"Setting Resource Link : {rl}");
                _id = rl.Id;
                LinkCreatorModel.Name = rl.name;
                LinkCreatorModel.ClassId = rl.classid;
                LinkCreatorModel.Description = rl.description;
                LinkCreatorModel.Recycle = rl.recycle;
                ObservableCollection<IHueObject> listsel = new ObservableCollection<IHueObject>();
                foreach (string s in rl.links)
                {
                    List<string> parts = s.Split('/').ToList();
                    parts.RemoveAt(0);    

                    IHueObject obj = ListHueObjects.First(x => x.Id == parts[1] && x.GetType() == HueObjectCreator.CreateHueObject(parts[0]).GetType());
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
            get => _resourceLinkCreatorModel;
            set => SetProperty(ref _resourceLinkCreatorModel,value);
        }

        public ObservableCollection<IHueObject> ListHueObjects
        {
            get => _listIHueObjects;
            set => SetProperty(ref _listIHueObjects, value);
        }

        [MinimumCount(1, ErrorMessageResourceType = typeof(GlobalStrings), ErrorMessageResourceName = "ResourceLinks_SelectAtLeastOne")]
        public ObservableCollection<IHueObject> SelectedLinkObjects
        {
            get => _selectedLinkObjects;
            set => SetProperty(ref _selectedLinkObjects,value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set { SetProperty(ref _isEditing,value); RaisePropertyChanged("BtnSaveText"); RaisePropertyChanged("NotEditing"); }
        }

        public bool NotEditing => !_isEditing;

    }
}
