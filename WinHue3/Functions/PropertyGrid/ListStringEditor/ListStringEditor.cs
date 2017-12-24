using System;
using System.ComponentModel;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace WinHue3.Functions.PropertyGrid.ListStringEditor
{
    class ListStringEditor : ITypeEditor
    {
        public ListStringEditor(Type type)
        {
        }

        public override string ToString()
        {
            return string.Join(",",new BindingList<string>());
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            throw new NotImplementedException();
        }
    }
}
