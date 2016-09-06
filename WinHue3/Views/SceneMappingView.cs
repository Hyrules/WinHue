using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HueLib2;

namespace WinHue3
{
    public class SceneMappingView : View
    {
        private DataTable _dt;
        private string _filter;
        private object _selectedcell;
        private object _row;

        public SceneMappingView()
        {
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

        public object Row
        {
            get
            {
                return _row;           
            }
            set
            {
                _row = value;
                OnPropertyChanged();
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

            CommandResult comres = BridgeStore.SelectedBridge.GetListObjects<Scene>();
            if (comres.Success)
            {
                if (!WinHueSettings.settings.ShowHiddenScenes)
                    lscenes = ((Dictionary<string, Scene>) comres.resultobject).Where(
                            x => x.Value.name.Contains("HIDDEN") == false)
                        .ToDictionary(p => p.Key, p => p.Value);
                else
                    lscenes = ((Dictionary<string, Scene>) comres.resultobject);

            }
            else
            {
                return;
            }

            CommandResult resscenes = BridgeStore.SelectedBridge.GetListObjects<Light>();
            if (resscenes.Success)
            {
                Dictionary<string, Light> llights = (Dictionary<string, Light>) resscenes.resultobject;

                DataTable dt = new DataTable();


                dt.Columns.Add("ID");
                dt.Columns.Add("Name");

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

                    object[] data = new object[llights.Count + 5];

                    data[0] = new string(svp.Key.ToCharArray());
                    data[1] = new string(svp.Value.name.ToCharArray());
                    int i = 2;
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
            else
            {
                MessageBoxError.ShowLastErrorMessages(BridgeStore.SelectedBridge);
            }
            
        }

        public void RefreshSceneMapping()
        {
            BuildSceneMapping();
            FilterData();
        }

        public void ProcessDoubleClick()
        {
            if (_row == null) return;
            BridgeStore.SelectedBridge.ActivateScene(((DataRowView) _row).Row.ItemArray[0].ToString());
        }

        public ICommand RefreshMappingCommand => new RelayCommand(param => RefreshSceneMapping());
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => ProcessDoubleClick());
    }
}