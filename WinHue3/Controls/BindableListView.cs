using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HueLib2;
using WinHue3.Annotations;

namespace WinHue3.Controls
{
    public class BindableListView : ListView
    {
        public BindableListView()
        {
            SelectionChanged += BindableListView_SelectionChangedEvent;
        }

        void BindableListView_SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            ObservableCollection<HueObject> listselected = new ObservableCollection<HueObject>();
            foreach (var i in SelectedItems)
            {
                listselected.Add((HueObject)i);
            }
            SelectedItemsList = listselected;
        }

        public ObservableCollection<HueObject> SelectedItemsList
        {
            get { return (ObservableCollection<HueObject>)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
        DependencyProperty.Register("SelectedItemsList", typeof(ObservableCollection<HueObject>), typeof(BindableListView), new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetCurrentValue(SelectedItemsListProperty, e.NewValue);
            BindableListView blv = d as BindableListView; 
            blv?.OnPropertyModified(e);
        }

        private void OnPropertyModified(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            ObservableCollection<HueObject> listsel = (ObservableCollection<HueObject>) e.NewValue;
            foreach (var i in listsel)
            {
                SelectedItems.Add(i);
            }
        }

    }
}
