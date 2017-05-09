using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinHue3.Utils;

namespace WinHue3.Controls
{
    public class BindableTreeView : TreeView
    {
        public BindableTreeView() : base()
        {
            this.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(OnSelectedItemChanged);
        }

        public object BindableSelectedItem
        {
            get { return (object) GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("BindableSelectedItem", typeof(object), typeof(BindableTreeView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (SelectedItem is RuleTreeViewItem)
            {
                RuleTreeViewItem rtvi = (RuleTreeViewItem) SelectedItem;
                SetValue(SelectedItemProperty, rtvi.Childrens == null ? SelectedItem : null);
            }
        }

        
    }
}