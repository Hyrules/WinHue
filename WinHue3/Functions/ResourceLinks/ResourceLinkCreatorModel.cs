using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using WinHue3.Utils;
using IHueObject = WinHue3.Philips_Hue.HueObjects.Common.IHueObject;

namespace WinHue3.Functions.ResourceLinks
{
    public class ResourceLinkCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private ObservableCollection<IHueObject> _listlinkObject;
        private string _description;
        private bool? _recycle;
        private ushort _classId;
        private bool _showId;
        private bool _wrap;

        public ResourceLinkCreatorModel()
        {
            _name = string.Empty;
            _listlinkObject = new ObservableCollection<IHueObject>();
            _classId = 1;
        }

        [StringLength(32,MinimumLength = 1,ErrorMessageResourceName = "ResourceLinks_NameNeeded", ErrorMessageResourceType = typeof(GlobalStrings))]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }
       
        public ObservableCollection<IHueObject> ListlinkObject
        {
            get => _listlinkObject;
            set => SetProperty(ref _listlinkObject,value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description,value);
        }

        public bool? Recycle
        {
            get => _recycle;
            set => SetProperty(ref _recycle,value);
        }

        public ushort ClassId
        {
            get => _classId;
            set => SetProperty(ref _classId,value);
        }

        public bool ShowID
        {
            get => _showId;
            set => SetProperty(ref _showId, value);
        }

        public bool Wrap
        {
            get => _wrap;
            set => SetProperty(ref _wrap,value);
        }
    }
}
