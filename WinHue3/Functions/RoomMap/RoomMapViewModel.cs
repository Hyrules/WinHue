using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GongSolutions.Wpf.DragDrop;
using Microsoft.Win32;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Utils;

namespace WinHue3.Functions.RoomMap
{
    public class RoomMapViewModel : ValidatableBindableBase, IDropTarget
    {
        private OpenFileDialog ofd;
        private ImageSource _floorPlan;
        private ObservableCollection<Light> _listLights;
        private ObservableCollection<HueElement> _listCanvasLights;
        private HueElement _selectedItem;

        public RoomMapViewModel()
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "JPEG Files (*.jpg)|*.jpg|Bitmap Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png";
            ofd.DefaultExt = "*.jpg";
            ListCanvasLights = new ObservableCollection<HueElement>();
        }

        public ICommand ChooseImageCommand => new RelayCommand(param => ChooseImage());
        public ICommand ClickObjectCommand => new RelayCommand(ClickObject);

        
        private void ClickObject(object obj)
        {
            SelectedItem = obj as HueElement;
        }

        public ImageSource FloorPlan
        {
            get => _floorPlan;
            set => SetProperty(ref _floorPlan,value);
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
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem,value); }
        }

        private void ChooseImage()
        {
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                FloorPlan = new BitmapImage(new Uri(ofd.FileName));
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
                ListCanvasLights[ListCanvasLights.IndexOf(dropInfo.Data as HueElement)].X = dropInfo.DropPosition.X;
                ListCanvasLights[ListCanvasLights.IndexOf(dropInfo.Data as HueElement)].Y = dropInfo.DropPosition.Y;
            }
            else
            {
                Light l = dropInfo.Data as Light;


                HueElement newElement = new HueElement(l)
                {
                    X = dropInfo.DropPosition.X,
                    Y = dropInfo.DropPosition.Y
                };


                ListCanvasLights.Add(newElement);
                ListLights.Remove(l);


            }
            
        }
    }
}
