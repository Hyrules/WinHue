using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;


namespace WinHue3.Controls.ListStringEditor
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
