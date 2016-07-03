using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HueLib;
using HueLib_base;

namespace WinHue3
{
    public class SceneMappingView : View
    {
        private Bridge _bridge;
        private DataTable _dt;
        private string _filter;

        public SceneMappingView(Bridge br)
        {
            _bridge = br;
            BuildSceneMapping();
        }

        public DataView SceneMapping => _dt.DefaultView;

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

        public void BuildSceneMapping()
        {
            Dictionary<string, Scene> lscenes;

            if (!WinHueSettings.settings.ShowHiddenScenes)
                lscenes = _bridge.GetScenesList()
                    .Where(x => x.Value.name.Contains("HIDDEN") == false)
                    .ToDictionary(p => p.Key, p => p.Value);
            else
                lscenes = _bridge.GetScenesList();

            Dictionary<string, Light> llights = _bridge.GetLightList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Scenes");

            // Add all light columns
            foreach (KeyValuePair<string, Light> kvp in llights)
            {
                dt.Columns.Add(kvp.Value.name);
            }

            dt.Columns.Add("Locked");
            dt.Columns.Add("Recycle");
            dt.Columns.Add("Version");

            // Map each scenes and lights
            foreach (KeyValuePair<string, Scene> svp in lscenes)
            {

                object[] data = new object[llights.Count + 4];
                data[0] = svp.Value.name;
                int i = 1;
                foreach (KeyValuePair<string, Light> lvp in llights)
                {
                    string value = svp.Value.lights.Contains(lvp.Key) ? "Assigned" : "";
                    data[i] = new string(value.ToCharArray());
                    i++;
                }
                data[i] = new string(svp.Value.locked.ToString().ToCharArray());
                data[i + 1] = new string(svp.Value.recycle.ToString().ToCharArray());
                data[i + 2] = new string(svp.Value.version.ToString().ToCharArray());

                dt.Rows.Add(data);
            }

            _dt = dt;
            OnPropertyChanged("SceneMapping");
            
        }

        public void RefreshSceneMapping()
        {
            BuildSceneMapping();
            FilterData();
        }

        public ICommand RefreshMappingCommand => new RelayCommand(param => RefreshSceneMapping());

    }
}