using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WinHue3.Properties;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace WinHue3.Functions.PropertyGrid
{
    /// <summary>
    /// Interaction logic for XYEditor.xaml
    /// </summary>
    public partial class XYEditor : UserControl, ITypeEditor, INotifyPropertyChanged
    {
        private decimal _max;
        private decimal _min;


        public XYEditor()
        {
            InitializeComponent();
            _max = 1.0m;
            _min = 0;
        }

        public decimal Max => _max;

        public decimal Min => _min;

        public decimal? X
        {
            get => Value?[0];
            set
            {
                if(Value == null) Value = new decimal[2];
                if (value == null)
                {
                    Value = null;
                }
                else
                {
                    Value[0] = (decimal)value;
                }
                
                OnPropertyChanged("Y");
                OnPropertyChanged();
            }
        }

        public decimal? Y
        {
            get => Value?[1];
            set
            {
                if (Value == null) Value = new decimal[2];
                if (value == null)
                {
                    Value = null;
                }
                else
                {
                    Value[1] = (decimal)value;
                }
                
                OnPropertyChanged("X");
                OnPropertyChanged();
            }
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(decimal[]), typeof(XYEditor), new PropertyMetadata(null));

        public decimal[] Value
        {
            get { return (decimal[])GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);

            }
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var binding = new Binding("Value")
            {
                Source = propertyItem,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };

            var mm = propertyItem.PropertyDescriptor.Attributes.OfType<MaxMinAttribute>();
            if (mm.Any())
            {
                MaxMinAttribute mma = mm.First();
                _max = Convert.ToDecimal(mma.Max);
                _min = Convert.ToDecimal(mma.Min);
            }

            BindingOperations.SetBinding(this, ValueProperty, binding);
            OnPropertyChanged("X");
            OnPropertyChanged("Y");
            OnPropertyChanged("Max");
            OnPropertyChanged("Min");
            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class MaxMinAttribute : Attribute
        {
            public string Max { get; }
            public string Min { get; }

            public MaxMinAttribute(string max, string min)
            {
                Max = max;
                Min = min;
            }
        }
    }



}
