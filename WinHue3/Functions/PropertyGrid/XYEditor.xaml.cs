using System.ComponentModel;
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


        public XYEditor()
        {
            InitializeComponent();

        }

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
            BindingOperations.SetBinding(this, ValueProperty, binding);
            OnPropertyChanged("X");
            OnPropertyChanged("Y");
            return this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
