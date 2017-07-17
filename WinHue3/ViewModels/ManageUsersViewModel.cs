using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HueLib2;
using WinHue3.Models;

namespace WinHue3.ViewModels
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
            get { return _manageUsersModel; }
            set { SetProperty(ref _manageUsersModel,value); }
        }

        public ObservableCollection<Whitelist> ListUsers
        {
            get { return _listUsers; }
            set { SetProperty(ref _listUsers, value); }
        }

        public Whitelist SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                SetProperty(ref _selectedUser, value);
                if (value == null) return;
                string[] userdev = _selectedUser.Name.Split('#');
                UsersModel.Created = _selectedUser.CreateDate;
                UsersModel.Lastused = _selectedUser.LastUseDate;
                UsersModel.Key = _selectedUser.id;
                UsersModel.ApplicationName = userdev[0];
                UsersModel.Devtype = userdev.Length > 1 ? userdev[1] : string.Empty;
            }
        }

        public Bridge Bridge
        {
            get { return _bridge; }
            set
            {
                SetProperty(ref _bridge, value);
                CommandResult<Dictionary<string,Whitelist>> cr = _bridge.GetUserList();
                if (cr.Success)
                {
                    Dictionary<string, Whitelist> list = cr.Data;

                    foreach (var item in list)
                    {
                        Whitelist i = item.Value;
                        i.id = item.Key;
                        if (i.id != _bridge.ApiKey)
                            ListUsers.Add(i);
                    }

                }
                else
                {
                    MessageBoxError.ShowLastErrorMessages(_bridge);
                }
            }
        }

        private bool CanDeleteUser()
        {
            return SelectedUser != null;
        }

        private void Delete()
        {
            CommandResult<bool> cr = _bridge.RemoveUser(UsersModel.Key);
            if (cr.Success)
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

        private void AddUser()
        {
            string uname = UsersModel.Devtype != string.Empty ? UsersModel.ApplicationName + "#" + UsersModel.Devtype : UsersModel.ApplicationName;
            CommandResult<string> cr = _bridge.CreateUser(uname);
            if (cr.Success)
            {
                Whitelist newitem = new Whitelist
                {
                    Name = uname,
                    id = cr.Data,
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


        public ICommand DeleteCommand => new RelayCommand(param => Delete(), (param)=> CanDeleteUser());
        public ICommand AddUserCommand => new RelayCommand(param => AddUser(), (param) => CanAddUser());
        public ICommand ClearCommand => new RelayCommand(param => Clear(), (param) => CanDeleteUser());




    }
}
