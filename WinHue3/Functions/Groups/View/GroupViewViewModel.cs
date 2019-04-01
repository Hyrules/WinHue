using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.ExtensionMethods;
using WinHue3.Functions.Groups.Creator;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;
using Group = WinHue3.Philips_Hue.HueObjects.GroupObject.Group;


namespace WinHue3.Functions.Groups.View
{
    public class GroupViewViewModel : ValidatableBindableBase, IDisposable
    {
        private List<Group> _groups;
        private List<Light> _lights;
        private DataTable _dt;
        private string _filter;
        private bool _reverse;
        private bool _disposed = false;
        private object _row;

        public GroupViewViewModel()
        {
            _dt = new DataTable();

        }


        public ICommand EditGroupCommand => new AsyncRelayCommand(param => EditGroup(), param => CanEditGroup());
        public ICommand RefreshGroupViewCommand => new RelayCommand(param => RefreshGroupMapping());

        private bool CanEditGroup()
        {
            return SelectedGroup != null;
        }

        private async Task EditGroup()
        {
            DataRowView drv = SelectedGroup as DataRowView;
            Form_GroupCreator fgc = new Form_GroupCreator();
            await fgc.Initialize(drv.Row.ItemArray[0].ToString());
            if(fgc.ShowDialog().GetValueOrDefault())
            {
                RefreshGroupMapping();
            }
        }

        private void RefreshGroupMapping()
        {
            if (Reverse)
            {
                BuildGroupView();
            }
            else
            {
                BuildGroupViewReverse();
            }
            
            FilterData();
        }

        public void Initialize(List<Group> groups, List<Light> lights)
        {

            _groups = groups;
            _lights = lights;
            BuildGroupViewReverse();;
        }

        public DataView GroupsDetails
        {
            get => _dt.DefaultView;

        } 

        public bool Reverse
        {
            get => _reverse;
            set
            {
                SetProperty(ref _reverse,value);
                if (value)
                {
                    BuildGroupView();
                }
                else
                {
                    BuildGroupViewReverse();
                }
            }
        }

        private void BuildGroupView()
        {
            _dt.Clear();
            List<Group> lgroups = _groups;
            List<Light> llights = _lights;
            if (lgroups == null) return;
            DataTable dt = new DataTable();

            dt.Columns.Add("Properties");
            foreach (Group gvp in lgroups)
            {
                dt.Columns.Add(gvp.name);
            }

            PropertyInfo[] listproperties = typeof(Group).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => !x.Name.Contains("name") && x.Name != "Image" && x.Name != "action").ToArray(); 
            PropertyInfo[] listaction = typeof(Action).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => !x.Name.Contains("_inc") && x.Name != "scene").ToArray(); 
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + listaction.Length];

            listproperties.CopyTo(listPropertyInfos,0);
            listaction.CopyTo(listPropertyInfos,listproperties.Length);

            object[] data = new object[dt.Columns.Count];

            
            foreach (Light l in llights)
            {
                int i = 0;
                data[i] = l.name;
                i++;
                foreach (Group g in lgroups)
                {
                    if (g.lights.Contains(l.Id))
                        data[i] = "Assigned";
                    else
                    {
                        data[i] = "";
                    }
                    i++;
                }
                dt.Rows.Add(data);
            }



            foreach (PropertyInfo pi in listPropertyInfos)
            {

                int i = 1;
                data[0] = pi.Name;

                foreach (Group g in lgroups)
                {
                    object value = null;
                    value = Array.Find(listaction, x => x.Name == pi.Name) != null ? pi.GetValue(g.action) : pi.GetValue(g);

                    if (value is Array)
                    {
                        value = ((Array)value).ArrayToString();
                    }

                    data[i] = value;
                    i++;
                }
              
 
                dt.Rows.Add(data);
            }
            _dt = dt;
            RaisePropertyChanged("GroupsDetails");
        }

        
        private void BuildGroupViewReverse()
        {
            List<Group> lgroups = _groups;
            List<Light> llights = _lights;
            if (lgroups == null) return;
            DataTable dt = new DataTable();
            dt.Columns.Add("Groups");

            PropertyInfo[] listproperties = typeof(Group).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => !x.Name.Contains("name") && x.Name != "Image" && x.Name != "action").ToArray();
            PropertyInfo[] listaction = typeof(Action).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => !x.Name.Contains("_inc") && x.Name != "scene").ToArray();
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + listaction.Length];

            listproperties.CopyTo(listPropertyInfos, 0);
            listaction.CopyTo(listPropertyInfos, listproperties.Length);

            foreach (Light l in llights)
            {
                dt.Columns.Add(l.name);
            }

            foreach (PropertyInfo pi in listPropertyInfos)
            {
                dt.Columns.Add(pi.Name);
            }

            object[] data = new object[dt.Columns.Count];
           
            foreach (Group g in lgroups)
            {
                int i = 1;
                
                data[0] = g.name;

                foreach (Light l in llights)
                {
                    if (g.lights.Contains(l.Id))
                        data[i] = "Assigned";
                    else
                        data[i] = "";
                    i++;
                }

                foreach (PropertyInfo pi in listPropertyInfos)
                {
                    object value = null;
                    value = Array.Find(listaction, x => x.Name == pi.Name) != null ? pi.GetValue(g.action) : pi.GetValue(g);

                    if (value is Array)
                    {
                        value = ((Array)value).ArrayToString();
                    }

                    data[i] = value;
                    i++;

                }
                Console.WriteLine(i);
                dt.Rows.Add(data);

            }
            _dt = dt;
            RaisePropertyChanged("GroupsDetails");

        }

        public string Filter
        {
            get => _filter;
            set
            {
                SetProperty(ref _filter,value);
                FilterData();

            }
        }

        public object SelectedGroup { get => _row; set => SetProperty(ref _row ,value); }

        public void FilterData()
        {
            if (_filter == string.Empty)
            {
                _dt.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                foreach (DataColumn column in _dt.Columns)
                {
                    sb.Append($"[{column.ColumnName}] Like '%{_filter}%' OR ");
                }

                sb.Remove(sb.Length - 3, 3);
                _dt.DefaultView.RowFilter = sb.ToString();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dt.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
