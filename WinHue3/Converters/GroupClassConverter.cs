using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WinHue3.Converters
{
    public class GroupClassConverter : IValueConverter
    {
        /*
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_LivingRoom}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Kitchen}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Dining}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Bedroom}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_KidsBedroom}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Bathroom}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Nursery}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Recreation}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Office}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Gym}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Hallway}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Toilet}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_FrontDoor}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Garage}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Terrace}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Garden}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_DriveWay}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Carport}"/>
            <ComboBoxItem Content="{x:Static resx:GUI.GroupCreatorForm_Class_Other}"/>*/


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string @class = value.ToString();
            switch (@class)
            {
                case "Living room":
                    return 0;
                case "Kitchen":
                    return 1;
                case "Dining":
                    return 2;
                case "Bedroom":
                    return 3;
                case "Kids bedroom":
                    return 4;
                case "Bathroom":
                    return 5;
                case "Nursery":
                    return 6;
                case "Recreation":
                    return 7;
                case "Office":
                    return 8;
                case "Gym":
                    return 9;
                case "Hallway":
                    return 10;
                case "Toilet:":
                    return 11;
                case "Front door":
                    return 12;
                case "Garage":
                    return 13;
                case "Terrace":
                    return 14;
                case "Garden":
                    return 15;
                case "Driveway":
                    return 16;
                case "Carport":
                    return 17;
                case "Other":
                    return 18;
                default:
                    return 18;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int) value;
            switch (index)
            {
                case 0:
                    return "Living room";
                case 1:
                    return "Kitchen";
                case 2:
                    return "Dining";
                case 3:
                    return "Bedroom";
                case 4:
                    return "Kids bedroom";
                case 5:
                    return "Bathroom";
                case 6:
                    return "Nursery";
                case 7:
                    return "Recreation";
                case 8:
                    return "Office";
                case 9:
                    return "Gym";
                case 10:
                    return "Hallway";
                case 11:
                    return "Toilet:";
                case 12:
                    return "Front door";
                case 13:
                    return "Garage";
                case 14:
                    return "Terrace";
                case 15:
                    return "Garden";
                case 16:
                    return "Driveway";
                case 17:
                    return "Carport";
                case 18:
                    return "Other";
                default:
                    return 18;
            }
        }
    }
}
