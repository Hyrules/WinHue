using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using HueLib2;
using Action = HueLib2.Action;
using Group = HueLib2.Group;

namespace WinHue3.ViewModels
{
    public class GroupViewViewModel : ValidatableBindableBase
    {
        private List<Group> _groups;
        private List<Light> _lights;
        private DataTable _dt;
        private string _filter;
        private bool _reverse;

        public GroupViewViewModel()
        {

        }

        public void Initialize(List<Group> groups, List<Light> lights)
        {
            _groups = groups;
            _lights = lights;
            BuildGroupViewReverse();
        }

        public DataView GroupsDetails => _dt?.DefaultView;

        public bool Reverse
        {
            get { return _reverse; }
            set
            {
                SetProperty(ref _reverse,value);
                if (value == true)
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
            List<Group> lgroups = _groups;
            List<Light> llights = _lights;
            if (lgroups == null) return;
            DataTable dt = new DataTable();

            dt.Columns.Add("Properties");
            foreach (Group gvp in lgroups)
            {
                dt.Columns.Add(gvp.Name);
            }

            PropertyInfo[] listproperties = typeof(Group).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            PropertyInfo[] listaction = typeof(Action).GetProperties();
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + listaction.Length];

            listproperties.CopyTo(listPropertyInfos,0);
            listaction.CopyTo(listPropertyInfos,listproperties.Length);

            object[] data = new object[lgroups.Count+1];

            
            foreach (Light l in llights)
            {
                int i = 0;
                data[i] = l.Name;
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
                if (pi.Name == "action" || pi.Name == "name" || pi.Name.Contains("_inc") || pi.Name == "lights" || pi.Name == "Image") continue;

                int i = 1;
                data[0] = pi.Name;

                foreach (Group g in lgroups)
                {
                    if (Array.Find(listaction,x => x.Name == pi.Name) != null)
                    {
                        data[i] = pi.GetValue(g.action);
                    }
                    else
                    {
                        data[i] = pi.GetValue(g);
                    }
                    
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

            PropertyInfo[] listproperties = typeof(Group).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            PropertyInfo[] liststate = typeof(Action).GetProperties();
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + liststate.Length];

            listproperties.CopyTo(listPropertyInfos, 0);
            liststate.CopyTo(listPropertyInfos, listproperties.Length);

            foreach (Light l in llights)
            {
                dt.Columns.Add(l.Name);
            }

            foreach (PropertyInfo pi in listPropertyInfos)
            {
                if (pi.Name == "action" || pi.Name == "name" || pi.Name.Contains("_inc") || pi.Name == "lights" || pi.Name == "Image") continue;
                dt.Columns.Add(pi.Name);
            }

            int nbrcol =  listPropertyInfos.Length - 2 - liststate.Count(x => x.Name.Contains("_inc")) + llights.Count;

            object[] data = new object[nbrcol];

            foreach (Group gvp in lgroups)
            {
                int i = 1;
                data[0] = gvp.Name;

                foreach (Light l in llights)
                {
                    if (gvp.lights.Contains(l.Id))
                        data[i] = "Assigned";
                    else
                        data[i] = "";
                    i++;
                }

                foreach (PropertyInfo pi in listPropertyInfos)
                {
                    if (pi.Name == "action" || pi.Name == "name" || pi.Name.Contains("_inc") || pi.Name == "lights" || pi.Name == "Image") continue;

                    if (Array.Find(liststate, x => x.Name == pi.Name) != null)
                    {
                        data[i] = pi.GetValue(gvp.action);
                    }
                    else
                    {
                        data[i] = pi.GetValue(gvp);
                    }
                    i++;

                }
                dt.Rows.Add(data);

            }
            _dt = dt;
            RaisePropertyChanged("GroupsDetails");
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                SetProperty(ref _filter,value);
                FilterData();

            }
        }

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

    }
}
