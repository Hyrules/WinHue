using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WinHue3.Functions.Behaviors
{
    public class HueObjectDragDropHelper : DependencyObject
    {
        public bool EnableDrop
        {
            get => (bool)GetValue(MyPropertyProperty);
            set => SetValue(MyPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("EnableDrop", typeof(bool), typeof(HueObjectDragDropHelper), new PropertyMetadata(OnDropEnable));

        private static void OnDropEnable(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            if (d is Control control) control.Drop += OnDrop;
        }

        private static void OnDrop(object _sender, DragEventArgs e)
        {

            DependencyObject d = _sender as DependencyObject;
            if (d == null) return;
            Object target = d.GetValue(FileDragDropTargetProperty);
        }


    }
}
