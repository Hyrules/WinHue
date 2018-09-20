using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Resources;
using WinHue3.Utils;


namespace WinHue3.Functions.RoomMap
{
    public class RoomMapViewModel : ValidatableBindableBase, IDropTarget
    {
        private OpenFileDialog ofd;
        private BitmapImage _floorPlanImage;
        private ObservableCollection<HueElement> _listLights;
        private ObservableCollection<HueElement> _listCanvasLights;
        private HueElement _selectedItem;
        private ObservableCollection<Floor> _listFloors;
        private string _floorPlanName;
        private Floor _selectedFloor;
        private double _canvasHeight;
        private double _canvasWidth;
        private Stretch _stretchMode;
        private bool _isSaved;

        public RoomMapViewModel()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "All Supported Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png";
            ofd.DefaultExt = "*.jpg;*.png;*.bmp";
            ListCanvasLights = new ObservableCollection<HueElement>();
            ListFloors = new ObservableCollection<Floor>(WinHueSettings.LoadFloorPlans());
            CanvasHeight = 0;
            CanvasWidth = 0;
            StretchMode = Stretch.None;
        }

        public ICommand CreateNewFloorPlanCommand => new RelayCommand(param => CreateNewFloorPlan());
        public ICommand SaveFloorPlanCommand => new RelayCommand(param => SaveFloorPlan(), (param) => CanSaveFloor());
        public ICommand SelectFloorPlanCommand => new RelayCommand(param => SelectFloorPlan());
        public ICommand DeleteFloorPlanCommand => new RelayCommand(param => DeleteFloorPlan(), (param) => CanDeleteFloorPlan());
        public ICommand DeleteHueElementCommand => new RelayCommand(DeleteHueElement, (param) => _selectedItem != null);
        public ICommand MakeAllSameSizeCommand => new RelayCommand(param => MakeAllSameSize(), (param) => CanMakeSameSize());
        public ICommand DeleteSelectedElementCommand => new RelayCommand(param => DeleteSelectedElement(), (param) => CanDeleteElement());
        
        private bool CanDeleteElement()
        {
            return SelectedItem != null;
        }

        private void DeleteSelectedElement()
        {
            if (SelectedItem == null) return;
            ListCanvasLights.Remove(_selectedItem);
            SelectedItem = null;
        }

        private void MakeAllSameSize()
        {
            if (_selectedItem == null) return;
            foreach (HueElement he in ListCanvasLights)
            {
                if(he == SelectedItem) continue;
                he.ImageHeight = SelectedItem.ImageHeight;
                he.ImageWidth = SelectedItem.ImageWidth;
                he.PanelHeight = SelectedItem.PanelHeight;
                he.PanelWidth = SelectedItem.PanelWidth;
                
            }
        }

        private bool CanMakeSameSize()
        {
            return _selectedItem != null;
        }

        private void DeleteHueElement(object obj)
        {
            KeyEventArgs kea = obj as KeyEventArgs;
            if (kea == null) return;
            if (kea.Key != Key.Delete) return;
         
        }


        private bool CanDeleteFloorPlan()
        {
            return _selectedFloor != null;
        }


        private void DeleteFloorPlan()
        {
            if (MessageBox.Show(string.Format(GUI.FormRoomMap_Delete,SelectedFloor.Name), GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            if (!WinHueSettings.DeleteFloorPlan(_selectedFloor)) return;
            
            ListFloors.Remove(_selectedFloor);
            SelectedFloor = null;
            ListCanvasLights.Clear();
            FloorPlanImage = null;
            SelectedItem = null;
            FloorPlanName = string.Empty;
            CanvasWidth =0;
            CanvasHeight = 0;
        }


        private bool CanSaveFloor()
        {
            if (string.IsNullOrEmpty(FloorPlanName) || string.IsNullOrWhiteSpace(FloorPlanName)) return false;
            return true;
        }
    
        private void SelectFloorPlan()
        {
            if (_selectedFloor == null) return;
            CanvasHeight = _selectedFloor.CanvasHeight;
            CanvasWidth = _selectedFloor.CanvasWidth;
            StretchMode = _selectedFloor.StretchMode;
            FloorPlanName = _selectedFloor.Name;
            FloorPlanImage = _selectedFloor.Image;
            ListCanvasLights = new ObservableCollection<HueElement>(_selectedFloor.Elements);
        }

        private void SaveFloorPlan()
        {
               
            Floor floorplan = new Floor()
            {
                Elements = _listCanvasLights.ToList(),
                Name = FloorPlanName,
                Image = _floorPlanImage,
                CanvasHeight = CanvasHeight,
                CanvasWidth = CanvasWidth
                
            };
            
            if (!WinHueSettings.SaveFloorPlan(floorplan))
            {
                
            }

        }

        public BitmapImage FloorPlanImage
        {
            get => _floorPlanImage;
            set => SetProperty(ref _floorPlanImage,value);
        }

        public ObservableCollection<HueElement> ListLights
        {
            get => _listLights;
            set => SetProperty(ref _listLights,value);
        }

        public ObservableCollection<HueElement> ListCanvasLights
        {
            get => _listCanvasLights;
            set => SetProperty(ref _listCanvasLights,value);
        }

        public HueElement SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem,value);
        }

        public ObservableCollection<Floor> ListFloors
        {
            get => _listFloors;
            set => SetProperty(ref _listFloors,value);
        }

        public string FloorPlanName
        {
            get => _floorPlanName;
            set => SetProperty(ref _floorPlanName,value);
        }

        public Floor SelectedFloor
        {
            get => _selectedFloor;
            set => SetProperty(ref _selectedFloor,value);
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set => SetProperty(ref _canvasHeight,value);
        }

        public double CanvasWidth
        {
            get => _canvasWidth;
            set => SetProperty(ref _canvasWidth,value);
        }

        public Stretch StretchMode
        {
            get => _stretchMode;
            set => SetProperty(ref _stretchMode,value);
        }

        private void CreateNewFloorPlan()
        {
            Form_CreateFloorPlan fcfp = new Form_CreateFloorPlan();
            if (fcfp.ShowDialog() != true) return;
            Floor newfloor = fcfp.GetNewFloorPlan();
            ListFloors.Add(newfloor);
            SelectedFloor = newfloor;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            HueElement he = dropInfo.Data as HueElement;
            if (he == null) return;

            ListBox lb = dropInfo.DragInfo.VisualSource as ListBox;


            if (lb.Name == "ListLightsGroups" && ListCanvasLights.Any(x=> x.Id == he.Id && x.HueType == he.HueType))
            {
                MessageBox.Show(GlobalStrings.ElementExists, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ListCanvasLights.Contains(he))
            {
                ListCanvasLights[ListCanvasLights.IndexOf(he)].X = dropInfo.DropPosition.X - (he.ImageWidth / 2);
                ListCanvasLights[ListCanvasLights.IndexOf(he)].Y = dropInfo.DropPosition.Y - (he.ImageHeight / 2);
            }
            else
            {
                
                he.X = dropInfo.DropPosition.X - (he.ImageWidth / 2);
                he.Y = dropInfo.DropPosition.Y - (he.ImageHeight / 2);

                ListCanvasLights.Add(he);
                ListLights.Remove(he);
            }
            
        }
    }
}
