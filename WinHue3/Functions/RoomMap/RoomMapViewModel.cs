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
using Color = System.Drawing.Color;


namespace WinHue3.Functions.RoomMap
{
    public class RoomMapViewModel : ValidatableBindableBase, IDropTarget
    {
        private OpenFileDialog ofd;
        private ObservableCollection<HueElement> _listLights;
        private HueElement _selectedItem;
        private ObservableCollection<Floor> _listFloors;
        private Floor _selectedFloor;
        private Floor _floorModel;
        private readonly List<string> _deleteFloor;

        public RoomMapViewModel()
        {
            ofd = new OpenFileDialog {Filter = "All Supported Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png", DefaultExt = "*.jpg;*.png;*.bmp"};
            ListFloors = new ObservableCollection<Floor>(WinHueSettings.LoadFloorPlans());
            _deleteFloor = new List<string>();
        }

        public ICommand CreateNewFloorPlanCommand => new RelayCommand(param => CreateNewFloorPlan());
        public ICommand SaveFloorPlanCommand => new RelayCommand(param => SaveFloorPlan(), (param) => CanSaveFloor());
        public ICommand SelectFloorPlanCommand => new RelayCommand(param => SelectFloorPlan());
        public ICommand DeleteFloorPlanCommand => new RelayCommand(param => DeleteFloorPlan(), (param) => CanDeleteFloorPlan());
        public ICommand DeleteHueElementCommand => new RelayCommand(DeleteHueElement, (param) => _selectedItem != null);
        public ICommand MakeAllSameSizeCommand => new RelayCommand(param => MakeAllSameSize(), (param) => CanMakeSameSize());
        public ICommand DeleteSelectedElementCommand => new RelayCommand(param => DeleteSelectedElement(), (param) => CanDeleteElement());
        public ICommand SaveAllFloorPlanCommand => new RelayCommand(param => SaveAllFloorPlan());
        public ICommand EditFloorPlanCommand => new RelayCommand(param => EditFloorPlan(), (param) => CanEditFloorPlan());

        private bool CanEditFloorPlan()
        {
            if (SelectedFloor == null) return false;
            return true;
        }

        private void EditFloorPlan()
        {
            Form_CreateFloorPlan fcfp = new Form_CreateFloorPlan(SelectedFloor);
            string oldfloor =  SelectedFloor.Name;

            if (fcfp.ShowDialog() == true)
            {
                
                Floor newfloor = fcfp.GetNewFloorPlan();
                SelectedFloor.CanvasHeight = newfloor.CanvasHeight;
                SelectedFloor.CanvasWidth = newfloor.CanvasWidth;
                SelectedFloor.Name = newfloor.Name;
                SelectedFloor.Image = newfloor.Image;
                SelectedFloor.StretchMode = newfloor.StretchMode;
                _deleteFloor.Add(oldfloor);
                
                
            }
        }


        private void SaveAllFloorPlan()
        {
            foreach (Floor f in ListFloors)
            {
                SaveFloor(f);
            }
        }

        private bool CanDeleteElement()
        {
            return SelectedItem != null;
        }

        private void DeleteSelectedElement()
        {
            if (SelectedItem == null) return;
            FloorModel.Elements.Remove(_selectedItem);
            SelectedItem = null;
        }

        private void MakeAllSameSize()
        {
            if (_selectedItem == null) return;
            foreach (HueElement he in FloorModel.Elements)
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

        private void SaveFloor(Floor floor)
        {
            if (WinHueSettings.SaveFloorPlan(floor))
            {
                floor.AcceptChanges();
                foreach (string s in _deleteFloor)
                {
                    WinHueSettings.DeleteFloorPlan(s);
                }
                _deleteFloor.Clear();
            }
            else
            {
                MessageBox.Show(GlobalStrings.ErrorSaving, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            SelectedItem = null;
       
        }


        private bool CanSaveFloor()
        {
            if (FloorModel == null) return false;
            if (string.IsNullOrEmpty(FloorModel.Name) || string.IsNullOrWhiteSpace(FloorModel.Name)) return false;
            if (!FloorModel.IsModified) return false;
            return true;
        }
    
        private void SelectFloorPlan()
        {
            if (_selectedFloor == null) return;
            FloorModel = SelectedFloor;
        }

        private void SaveFloorPlan()
        {
            SaveFloor(FloorModel);  

        }

        public ObservableCollection<HueElement> ListLights
        {
            get => _listLights;
            set => SetProperty(ref _listLights,value);
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


        public Floor SelectedFloor
        {
            get => _selectedFloor;
            set => SetProperty(ref _selectedFloor,value);
        }

        public Floor FloorModel
        {
            get => _floorModel;
            set => SetProperty(ref _floorModel,value);
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
            

            if (lb.Name == "ListLightsGroups" && FloorModel.Elements.Any(x=> x.Id == he.Id && x.HueType == he.HueType))
            {
                MessageBox.Show(GlobalStrings.ElementExists, GlobalStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (FloorModel.Elements.Contains(he))
            {
                FloorModel.Elements[FloorModel.Elements.IndexOf(he)].X = dropInfo.DropPosition.X - (he.ImageWidth / 2);
                FloorModel.Elements[FloorModel.Elements.IndexOf(he)].Y = dropInfo.DropPosition.Y - (he.ImageHeight / 2);
            }
            else
            {
                
                he.X = dropInfo.DropPosition.X - (he.ImageWidth / 2);
                he.Y = dropInfo.DropPosition.Y - (he.ImageHeight / 2);

                FloorModel.Elements.Add(he);
                ListLights.Remove(he);
            }
            
        }
    }
}
