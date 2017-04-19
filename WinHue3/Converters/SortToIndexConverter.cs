using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WinHue3.Models;

namespace WinHue3.Converters
{
    public class SortToIndexConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return -1;
            switch ((MainFormModel.WinHueSortOrder)value)
            {
                case MainFormModel.WinHueSortOrder.Default:
                    return 0;
                case MainFormModel.WinHueSortOrder.Ascending:
                    return 1;
                case MainFormModel.WinHueSortOrder.Descending:
                    return 2;
                default:
                    return 0;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == -1) return null;
            switch ((int)value)
            {
                case 0:
                    return MainFormModel.WinHueSortOrder.Default;
                case 1:
                    return MainFormModel.WinHueSortOrder.Ascending;
                case 2:
                    return MainFormModel.WinHueSortOrder.Descending;
                default:
                    goto case 0;

            }


        }
    }
}
