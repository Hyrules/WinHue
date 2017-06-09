using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HueLib2;

namespace WinHue3.Utils
{
    public class RuleTreeViewItem
    {
        private List<RuleTreeViewItem> _childrens;
        private string _name;
        private PropertyInfo _prop;
        private object _hueObject;
        private string _path;

        public RuleTreeViewItem()
        {
            Name = string.Empty;
            _childrens = new List<RuleTreeViewItem>();
        }

        public List<RuleTreeViewItem> Childrens
        {
            get { return _childrens; }
            set { _childrens = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public PropertyInfo Property
        {
            get { return _prop; }
            set { _prop = value; }
        }

        public object HObject
        {
            get { return _hueObject; }
            set { _hueObject = value; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
    }
}
