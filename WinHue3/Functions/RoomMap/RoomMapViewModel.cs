using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GongSolutions.Wpf.DragDrop;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Resources;
using WinHue3.Utils;
using File = System.IO.File;

namespace WinHue3.Functions.RoomMap
{
    public class RoomMapViewModel : ValidatableBindableBase, IDropTarget
    {
        private OpenFileDialog ofd;
        private BitmapImage _floorPlanImage;
        private ObservableCollection<Light> _listLights;
        private ObservableCollection<HueElement> _listCanvasLights;
        private HueElement _selectedItem;
        private ObservableCollection<Floor> _listFloors;
        private string _floorPlanName;
        private Floor _selectedFloor;

        public RoomMapViewModel()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "All Supported Image Files (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png";
            ofd.DefaultExt = "*.jpg;*.png;*.bmp";
            ListCanvasLights = new ObservableCollection<HueElement>();
            ListFloors = new ObservableCollection<Floor>(WinHueSettings.LoadFloorPlans());
        }

        public ICommand ChooseImageCommand => new RelayCommand(param => ChooseImage());
        public ICommand ClickObjectCommand => new RelayCommand(ClickObject);
        public ICommand SaveFloorPlanCommand => new RelayCommand(param => SaveFloorPlan(), (param) => CanSaveFloor());
        public ICommand SelectFloorPlanCommand => new RelayCommand(param => SelectFloorPlan());
        public ICommand ResizeWindowCommand => new RelayCommand(ResizeWindow);
        public ICommand DeleteFloorPlanCommand => new RelayCommand(param => DeleteFloorPlan(), (param) => CanDeleteFloorPlan());

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
        }


        private bool CanSaveFloor()
        {
            if (string.IsNullOrEmpty(FloorPlanName) || string.IsNullOrWhiteSpace(FloorPlanName)) return false;
            return true;
        }

        private void ResizeWindow(object obj)
        {
            SizeChangedEventArgs e = obj as SizeChangedEventArgs;
            double diffHeight = e.PreviousSize.Height - e.NewSize.Height;
            double diffWidth = e.PreviousSize.Width - e.NewSize.Width;

            foreach (HueElement h in _listCanvasLights)
            {
                h.X -= diffWidth;
                h.Y -= diffHeight;
            }
        }
    
        private void SelectFloorPlan()
        {
            if (_selectedFloor == null) return;
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
                Image = _floorPlanImage
            };
            
            if (!WinHueSettings.SaveFloorPlan(floorplan))
            {
                
            }




        }

        private void ClickObject(object obj)
        {
            SelectedItem = obj as HueElement;
        }

        public BitmapImage FloorPlanImage
        {
            get => _floorPlanImage;
            set => SetProperty(ref _floorPlanImage,value);
        }

        public ObservableCollection<Light> ListLights
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

        private void ChooseImage()
        {
            if (ofd.ShowDialog().GetValueOrDefault())
            {              
                FloorPlanImage = new BitmapImage(new Uri(ofd.FileName));
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {


            if (dropInfo.Data is HueElement h)
            {
                ListCanvasLights[ListCanvasLights.IndexOf(dropInfo.Data as HueElement)].X = dropInfo.DropPosition.X - (h.ImageWidth / 2);
                ListCanvasLights[ListCanvasLights.IndexOf(dropInfo.Data as HueElement)].Y = dropInfo.DropPosition.Y - (h.ImageHeight / 2);
            }
            else
            {
                Light l = dropInfo.Data as Light;


                HueElement newElement = new HueElement(l);
                newElement.X = dropInfo.DropPosition.X - (newElement.ImageWidth / 2);
                newElement.Y = dropInfo.DropPosition.Y - (newElement.ImageHeight / 2);

                ListCanvasLights.Add(newElement);
                ListLights.Remove(l);


            }
            
        }
    }
}
