using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;
using WinHue3.Models;

namespace WinHue3.ViewModels
{
    public class GroupCreatorViewModel : ValidatableBindableBase
    {
        private GroupCreatorModel _groupCreator;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _id;

        public GroupCreatorViewModel()
        {
            _groupCreator = new GroupCreatorModel();
            _id = string.Empty;
        }

        public Group Group
        {
            set
            {
                Group gr = value;
                GroupCreator.Name = gr.name;
                _id = gr.Id;
                ObservableCollection<Light> list = new ObservableCollection<Light>();
                foreach (string s in gr.lights)
                {
                    if (GroupCreator.ListAvailableLights.Any(x => x.Id == s))
                        list.Add(GroupCreator.ListAvailableLights.Single(x => x.Id == s));
                    else
                    {
                        log.Error($"Light ID:{s} does not seems to exists anymore.");
                    }
                }
                GroupCreator.Listlights = list;
            }
            get
            {
                Group gr = new Group {name = GroupCreator.Name, type = GroupCreator.Type, lights = GroupCreator.Listlights.Select(x => x.Id).ToList()};
                if (_id != string.Empty)
                    gr.Id = _id;
                if (GroupCreator.Type == "Room")
                    gr.@class = GroupCreator.Class;
                return gr;
            }
        }

        

        public GroupCreatorModel GroupCreator
        {
            get { return _groupCreator; }
            set { SetProperty(ref _groupCreator, value); }
        }

        private void ClearFields()
        {   
            GroupCreator.Listlights.Clear();
            GroupCreator.Name = string.Empty;
            GroupCreator.Type = "LightGroup";
            GroupCreator.Class = "Other";

        }

        public ICommand ClearFieldsCommand => new RelayCommand(param => ClearFields());


    }
}
