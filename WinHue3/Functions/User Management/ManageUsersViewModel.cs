using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.BridgeObject.BridgeObjects;
using WinHue3.Utils;

namespace WinHue3.Functions.User_Management
{
    public class ManageUsersViewModel : ValidatableBindableBase
    {
        private ManageUsersModel _manageUsersModel;
        private ObservableCollection<Whitelist> _listUsers;
        private Whitelist _selectedUser;

        private Bridge _bridge;

        public ManageUsersViewModel()
        {
            _manageUsersModel = new ManageUsersModel();
            _listUsers = new ObservableCollection<Whitelist>();
        }

        public ManageUsersModel UsersModel
        {
            get => _manageUsersModel;
            set => SetProperty(ref _manageUsersModel,value);
        }

        public ObservableCollection<Whitelist> ListUsers
        {
            get => _listUsers;
            set => SetProperty(ref _listUsers, value);
        }

        public Whitelist SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                if (value == null) return;
                string[] userdev = _selectedUser.Name.Split('#');
                UsersModel.Created = _selectedUser.CreateDate;
                UsersModel.Lastused = _selectedUser.LastUseDate;
                UsersModel.Key = _selectedUser.Id;
                UsersModel.ApplicationName = userdev[0];
                UsersModel.Devtype = userdev.Length > 1 ? userdev[1] : string.Empty;
            }
        }

        public async Task Initialize(Bridge bridge)
        {
            _bridge = bridge;
            Dictionary<string, Whitelist> cr = await _bridge.GetUserListAsyncTask();
            if (cr != null)
            {

                foreach (KeyValuePair<string, Whitelist> item in cr)
                {
                    Whitelist i = item.Value;
                    i.Id = item.Key;
                    if (i.Id != _bridge.ApiKey)
                        ListUsers.Add(i);
                }

            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
        }

        private bool CanDeleteUser()
        {
            return SelectedUser != null;
        }

        [Obsolete]
        private async Task Delete()
        {
            bool cr = await _bridge.RemoveUserAsyncTask(UsersModel.Key);
            if (cr)
            {
                ListUsers.Remove(SelectedUser);
                Clear();
            }
            else
                MessageBoxError.ShowLastErrorMessages(_bridge);
        }

        private bool CanAddUser()
        {
            return SelectedUser == null;
        }

        private async Task AddUser()
        {
            string uname = UsersModel.Devtype != string.Empty ? UsersModel.ApplicationName + "#" + UsersModel.Devtype : UsersModel.ApplicationName;
            string cr = await _bridge.CreateUserAsyncTask(uname);
            if (cr != null)
            {
                Whitelist newitem = new Whitelist
                {
                    Name = uname,
                    Id = cr,
                    CreateDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };
                ListUsers.Add(newitem);
                Clear();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
        }

        private void Clear()
        {
            UsersModel.ApplicationName = string.Empty;
            UsersModel.Created = string.Empty;
            UsersModel.Devtype = string.Empty;
            UsersModel.Key = string.Empty;
            UsersModel.Lastused = string.Empty;
            SelectedUser = null;
        }

        //public ICommand DeleteCommand => new AsyncRelayCommand(param => Delete(), param => CanDeleteUser());
        public ICommand AddUserCommand => new AsyncRelayCommand(param => AddUser(), param => CanAddUser());
        public ICommand ClearCommand => new RelayCommand(param => Clear(), (param) => CanDeleteUser());




    }
}
