using System.Windows;
using System.Windows.Interactivity;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace WinHue3.Functions.Rules.Creator
{
    public class PropertyGridBehavior : Behavior<Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid>
    {

        public PropertyItem SelectedPropertyItem
        {
            get => (PropertyItem)GetValue(SelectedPropertyItemProperty);
            set => SetValue(SelectedPropertyItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPropertyItemProperty =
            DependencyProperty.Register("SelectedPropertyItem", typeof(PropertyItem), typeof(PropertyGridBehavior), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedPropertyItemChanged));

        private static void OnSelectedPropertyItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid item)
            {
                item.SetValue(PropertyItemBase.IsSelectedProperty, true);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedPropertyItemChanged += AssociatedObject_SelectedPropertyItemChanged;
        }

        private void AssociatedObject_SelectedPropertyItemChanged(object sender, RoutedPropertyChangedEventArgs<PropertyItemBase> e)
        {
            this.SelectedPropertyItem = e.NewValue as PropertyItem;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectedPropertyItemChanged -= AssociatedObject_SelectedPropertyItemChanged;
        }


    }
}
