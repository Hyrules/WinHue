using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WinHue3.CustomControls
{
    public class BindableSlider : Slider
    {
        public static readonly DependencyProperty ValueChangedCommandProperty = DependencyProperty.RegisterAttached("ValueChangedCommand", typeof(ICommand), typeof(BindableSlider), new UIPropertyMetadata((s, e) =>
        {
            var element = s as Slider;

            if (element != null)
            {
                element.ValueChanged -= OnSingleValueChanged;

                if (e.NewValue != null)
                {
                    element.ValueChanged += OnSingleValueChanged;
                }
            }
        }));

        public static ICommand GetValueChangedCommand(UIElement element)
        {
            return (ICommand)element.GetValue(ValueChangedCommandProperty);
        }

        public static void SetValueChangedCommand(UIElement element, ICommand value)
        {
            element.SetValue(ValueChangedCommandProperty, value);
        }

        private static void OnSingleValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var element = sender as Slider;
            var command = element.GetValue(ValueChangedCommandProperty) as ICommand;

            if (command != null && command.CanExecute(element))
            {
                command.Execute(element);
                
                e.Handled = true;
            }
        }


    }
}
