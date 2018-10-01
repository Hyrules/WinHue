using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WinHue3.Functions.Application_Settings.Settings;

namespace WinHue3.Functions.RoomMap
{
    /// <summary>
    /// Interaction logic for Form_CreateFloorPlan.xaml
    /// </summary>
    public partial class Form_CreateFloorPlan : Window
    {
        private CreateFloorPlanViewModel fpvm;

        public Form_CreateFloorPlan()
        {
            InitializeComponent();
            fpvm = DataContext as CreateFloorPlanViewModel;
        }

        public Form_CreateFloorPlan(Floor floor) : this()
        {
            fpvm.FloorPlanName = floor.Name;
            fpvm.Height = floor.CanvasHeight;
            fpvm.Width = floor.CanvasWidth;
            fpvm.StretchMode = floor.StretchMode;
            fpvm.Image = floor.Image;
            if (fpvm.Image.Height == fpvm.Height && fpvm.Image.Width == fpvm.Width)
            {
                fpvm.UseImageSize = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public Floor GetNewFloorPlan()
        {
            Floor newfloor = new Floor();
            if (fpvm.ImagePath != string.Empty)
            {
                newfloor.Image = new BitmapImage(new Uri(fpvm.ImagePath));
            }
            else
            {
                if (fpvm.Image != null)
                {
                    newfloor.Image = (BitmapImage)fpvm.Image;
                }
            }
            
            newfloor.CanvasHeight = fpvm.Height;
            newfloor.CanvasWidth = fpvm.Width;
            newfloor.Name = fpvm.FloorPlanName;
            return newfloor;
        }

        private void btnCreateFloor_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult overwrite = MessageBoxResult.Yes;
            if (WinHueSettings.FloorPlanExists(fpvm.FloorPlanName))
            {
                overwrite = MessageBox.Show(GlobalStrings.FloorPlanExists, GlobalStrings.Warning, MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            if (overwrite == MessageBoxResult.No) return;
            DialogResult = true;
            Close();

        }
    }
}
