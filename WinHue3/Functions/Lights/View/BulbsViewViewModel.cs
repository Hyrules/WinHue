using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.Lights.View
{
    public class BulbsViewViewModel : ValidatableBindableBase
    {
        private DataTable _dt;
        private string _filter;
        private bool _reverse;
        private List<Light> _listlights;

        public BulbsViewViewModel()
        {

        }

        public void Initialize(List<Light> lights)
        {
            Listlights = lights;
            BuildBulbsViewReverse();
        }

        public DataView BulbsDetails => _dt?.DefaultView;

        public bool Reverse
        {
            get => _reverse;
            set
            {
                SetProperty(ref _reverse,value);
                if (value == true)
                {
                    BuildBulbsView();
                }
                else
                {
                    BuildBulbsViewReverse();
                }
            }
        }

        private void BuildBulbsView()
        {


            List<Light> llights = Listlights;
            DataTable dt = new DataTable();

            dt.Columns.Add("Properties");
            foreach (Light lvp in llights)
            {
                dt.Columns.Add(lvp.name);
            }

            PropertyInfo[] listproperties = typeof(Light).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList().Where(x => !x.Name.Contains("name") && x.Name != "Image" && x.Name != "state").ToArray();
            PropertyInfo[] liststate = typeof(State).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList().Where(x => !x.Name.Contains("_inc")).ToArray();
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + liststate.Length];

            listproperties.CopyTo(listPropertyInfos, 0);
            liststate.CopyTo(listPropertyInfos, listproperties.Length);

            object[] data = new object[dt.Columns.Count];

            foreach (PropertyInfo pi in listPropertyInfos)
            {

                data[0] = pi.Name;

                int i = 1;
                foreach (Light l in llights)
                {
                    object value = null;
                    value = Array.Find(liststate, x => x.Name == pi.Name) != null ? pi.GetValue(l.state) : pi.GetValue(l);

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
            RaisePropertyChanged("BulbsDetails");

        }


        private void BuildBulbsViewReverse()
        {

            List<Light> llights = Listlights;
            if (llights == null) return;
            DataTable dt = new DataTable();
            dt.Columns.Add("Lights");

            PropertyInfo[] listproperties = typeof(Light).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList().Where(x => !x.Name.Contains("name") && x.Name != "Image" && x.Name != "state").ToArray();
            PropertyInfo[] liststate = typeof(State).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).ToList().Where(x => !x.Name.Contains("_inc")).ToArray();
            
            PropertyInfo[] listPropertyInfos = new PropertyInfo[listproperties.Length + liststate.Length];

            listproperties.CopyTo(listPropertyInfos, 0);
            liststate.CopyTo(listPropertyInfos, listproperties.Length);

            foreach (PropertyInfo pi in listPropertyInfos)
            {
               
                dt.Columns.Add(pi.Name);
            }

            object[] data = new object[dt.Columns.Count];

            foreach (Light l in llights)
            {
                int i = 1;
                data[0] = l.name;

                foreach (PropertyInfo pi in listPropertyInfos)
                {

                    object value = null;
                    value = Array.Find(liststate, x => x.Name == pi.Name) != null ? pi.GetValue(l.state) : pi.GetValue(l);

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
            RaisePropertyChanged("BulbsDetails");

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

        public List<Light> Listlights
        {
            get => _listlights;

            set => SetProperty(ref _listlights,value);
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
