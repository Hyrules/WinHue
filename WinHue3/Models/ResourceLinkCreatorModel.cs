using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueLib2;
using WinHue3.Validation;

namespace WinHue3.Models
{
    public class ResourceLinkCreatorModel : ValidatableBindableBase
    {
        private string _name;
        private ObservableCollection<HueObject> _listlinkObject;
        private string _description;
        private bool? _recycle;
        private ushort _classId;
        private bool _showId;
        private bool _wrap;

        public ResourceLinkCreatorModel()
        {
            _name = string.Empty;
            ListlinkObject = new ObservableCollection<HueObject>();
            ClassId = 1;
        }

        [StringLength(32,MinimumLength = 1,ErrorMessageResourceName = "ResourceLinks_NameNeeded", ErrorMessageResourceType = typeof(GlobalStrings))]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name,value); }
        }
       
        public ObservableCollection<HueObject> ListlinkObject
        {
            get { return _listlinkObject; }
            set { SetProperty(ref _listlinkObject,value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description,value); }
        }

        public bool? Recycle
        {
            get { return _recycle; }
            set { SetProperty(ref _recycle,value); }
        }

        public ushort ClassId
        {
            get { return _classId; }
            set { SetProperty(ref _classId,value); }
        }

        public bool ShowID
        {
            get { return _showId; }
            set { SetProperty(ref _showId, value); }
        }

        public bool Wrap
        {
            get { return _wrap; }
            set { SetProperty(ref _wrap,value); }
        }
    }
}
