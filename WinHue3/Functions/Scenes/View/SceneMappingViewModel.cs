using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Functions.Scenes.Creator;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Utils;
using Application = System.Windows.Application;

namespace WinHue3.Functions.Scenes.View
{
    public class SceneMappingViewModel : ValidatableBindableBase
    {
        private DataTable _dt;
        private string _filter;
        private object _row;
        private List<Scene> _listscenes;
        private List<Light> _listlights;
        private Bridge _bridge;

        public SceneMappingViewModel()
        {

        }

        public void Initialize(Bridge bridge,List<Scene> scenes, List<Light> lights)
        {
            _bridge = bridge;
            _listscenes = scenes;
            _listlights = lights;
            BuildSceneMapping();
        }

        public DataView SceneMapping => _dt?.DefaultView;

        public string Filter
        {
            get => _filter;
            set
            {
                SetProperty(ref _filter,value);
                FilterData();
                
            }
        }

        public object Row
        {
            get => _row;
            set => SetProperty(ref _row,value);
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
            List<Scene> lscenes;

            if (!WinHueSettings.settings.ShowHiddenScenes)
                lscenes = _listscenes.Where(x => x.name.Contains("HIDDEN") == false).ToList();
            else
                lscenes = _listscenes;

            List<Light> llights = _listlights;

            DataTable dt = new DataTable();


            dt.Columns.Add("ID");
            dt.Columns.Add("Name");

            // Add all light columns
            foreach (Light kvp in llights)
            {
                dt.Columns.Add(kvp.name);
            }

            dt.Columns.Add("Locked");
            dt.Columns.Add("Recycle");
            dt.Columns.Add("Version");

            // Map each scenes and lights
            foreach (Scene svp in lscenes)
            {

                object[] data = new object[llights.Count + 5];

                data[0] = new string(svp.Id.ToCharArray());
                data[1] = new string(svp.name.ToCharArray());
                int i = 2;
                foreach (Light lvp in llights)
                {
                    string value = svp.lights.Contains(lvp.Id) ? "Assigned" : "";

                    data[i] = new string(value.ToCharArray());
                    i++;
                }
                data[i] = new string(svp.locked.ToString().ToCharArray());
                data[i + 1] = new string(svp.recycle.ToString().ToCharArray());
                data[i + 2] = new string(svp.version.ToString().ToCharArray());

                dt.Rows.Add(data);
            }


            _dt = dt;
            RaisePropertyChanged("SceneMapping");
            
            
        }

        public void RefreshSceneMapping()
        {
            BuildSceneMapping();
            FilterData();
        }

        public void ProcessDoubleClick()
        {
            if (_row == null) return;
            _bridge.ActivateScene(((DataRowView) _row).Row.ItemArray[0].ToString());
        }

        private bool ObjectSelected()
        {
            return Row != null;
        }

       
        public ICommand RefreshMappingCommand => new RelayCommand(param => RefreshSceneMapping(), (param) => ObjectSelected());
        public ICommand DoubleClickObjectCommand => new RelayCommand(param => ProcessDoubleClick(), (param) => ObjectSelected());

        public ICommand EditSceneCommand => new AsyncRelayCommand(param => EditScene(), param => CanEditScene());

        private bool CanEditScene()
        {
            return Row != null;
        }

        private async Task EditScene()
        {
            DataRowView drv = Row as DataRowView;
            Form_SceneCreator fsc = new Form_SceneCreator();
            fsc.Owner = Application.Current.MainWindow;
            await fsc.Inititalize(_bridge,drv.Row.ItemArray[0].ToString());
            if (fsc.ShowDialog().GetValueOrDefault())
            {
                RefreshSceneMapping();
            }
        }
    }
}