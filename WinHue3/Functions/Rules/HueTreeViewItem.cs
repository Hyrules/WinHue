using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WinHue3.Annotations;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Functions.Rules
{
    public sealed class HuePropertyTreeViewItem : TreeViewItem, INotifyPropertyChanged
    {
        private Type _propType;
        private HueAddress _address;
        private PropertyInfo _propInfo;

        public HuePropertyTreeViewItem() : base()
        {
           
        }

        public Type PropType
        {
            get => _propType;
            set => SetProperty(ref _propType, value);
        }

        public HueAddress Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public PropertyInfo PropInfo
        {
            get => _propInfo;
            set => SetProperty(ref _propInfo,value);
        }

        public List<HuePropertyTreeViewItem> ToList()
        {
            List<HuePropertyTreeViewItem> list = new List<HuePropertyTreeViewItem>();
            HuePropertyTreeViewItem[] arr = new HuePropertyTreeViewItem[Items.Count];
            Items.CopyTo(arr, 0);

            foreach (var i in arr)
            {
                Items.Remove(i);
                list.Add(i);
            }

            return list;
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
