using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using HueLib2;
using Action = HueLib2.Action;
using Group = HueLib2.Group;

namespace WinHue3
{
    public class GroupViewView : View
    {
        private Dictionary<string, Group> _groups;
        private Dictionary<string, Light> _lights;
        private DataTable _dt;
        private string _filter;
        private bool _reverse;

        public GroupViewView(Dictionary<string, Group> groups, Dictionary<string,Light> lights  )
        {
            _groups = groups;
            _lights = lights;
            BuildGroupViewReverse();
        }

        public DataView GroupsDetails => _dt.DefaultView;

        public bool Reverse
        {
            get { return _reverse; }
            set
            {
                _reverse = value;
                if (value == true)
                {
                    BuildGroupView();
                }
                else
                {
                    BuildGroupViewReverse();
                }
                OnPropertyChanged();
            }
        }

        private void BuildGroupView()
        {
            Dictionary<string, Group> lgroups = _groups;
            Dictionary<string, Light> llights = _lights;
            if (lgroups == null) return;
            DataTable dt = new DataTable();

            dt.Columns.Add("Properties");
            foreach (KeyValuePair<string,Group> gvp in lgroups)
            {
                dt.Columns.Add(gvp.Value.name);
            }



            PropertyInfo[] listproperties = typeof(Group).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            PropertyInfo[] listaction = typeof(Action).GetProperties();
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + listaction.Length];

            listproperties.CopyTo(listPropertyInfos,0);
            listaction.CopyTo(listPropertyInfos,listproperties.Length);

            object[] data = new object[lgroups.Count+1];

            
            foreach (KeyValuePair<string, Light> lvp in llights)
            {
                int i = 0;
                data[i] = lvp.Value.name;
                i++;
                foreach (KeyValuePair<string, Group> gvp in lgroups)
                {
                    if (gvp.Value.lights.Contains(lvp.Key))
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
                if (pi.Name == "action" || pi.Name == "name" || pi.Name.Contains("_inc") || pi.Name == "lights") continue;

                int i = 1;
                data[0] = pi.Name;

                
                foreach (KeyValuePair<string, Group> gvp in lgroups)
                {
                    if (Array.Find(listaction,x => x.Name == pi.Name) != null)
                    {
                        data[i] = pi.GetValue(gvp.Value.action);
                    }
                    else
                    {
                        data[i] = pi.GetValue(gvp.Value);
                    }
                    
                    i++;
                }
              
 
                dt.Rows.Add(data);
            }
            _dt = dt;
            OnPropertyChanged("GroupsDetails");
        }

        private void BuildGroupViewReverse()
        {
            Dictionary<string, Group> lgroups = _groups;
            Dictionary<string, Light> llights = _lights;
            if (lgroups == null) return;
            DataTable dt = new DataTable();
            dt.Columns.Add("Groups");

            PropertyInfo[] listproperties = typeof(Group).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            PropertyInfo[] liststate = typeof(Action).GetProperties();
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + liststate.Length];

            listproperties.CopyTo(listPropertyInfos, 0);
            liststate.CopyTo(listPropertyInfos, listproperties.Length);

            foreach (KeyValuePair<string, Light> lvp in llights)
            {
                dt.Columns.Add(lvp.Value.name);
            }

            foreach (PropertyInfo pi in listPropertyInfos)
            {
                if (pi.Name == "action" || pi.Name == "name" || pi.Name.Contains("_inc") || pi.Name == "lights") continue;
                dt.Columns.Add(pi.Name);
            }

            int nbrcol =  listPropertyInfos.Length - 2 - liststate.Count(x => x.Name.Contains("_inc")) + llights.Count;

            object[] data = new object[nbrcol];

            foreach (KeyValuePair<string,Group> gvp in lgroups)
            {
                int i = 1;
                data[0] = gvp.Value.name;

                foreach (KeyValuePair<string, Light> lvp in llights)
                {
                    if (gvp.Value.lights.Contains(lvp.Key))
                        data[i] = "Assigned";
                    else
                        data[i] = "";
                    i++;
                }

                foreach (PropertyInfo pi in listPropertyInfos)
                {
                    if (pi.Name == "action" || pi.Name == "name" || pi.Name.Contains("_inc") || pi.Name == "lights") continue;

                    if (Array.Find(liststate, x => x.Name == pi.Name) != null)
                    {
                        data[i] = pi.GetValue(gvp.Value.action);
                    }
                    else
                    {
                        data[i] = pi.GetValue(gvp.Value);
                    }
                    i++;

                }
                dt.Rows.Add(data);

            }
            _dt = dt;
            OnPropertyChanged("GroupsDetails");
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                OnPropertyChanged();
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
