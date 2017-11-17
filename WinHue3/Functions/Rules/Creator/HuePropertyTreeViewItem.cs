using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WinHue3.Utils;

namespace WinHue3.Functions.Rules.Creator
{
    public class HuePropertyTreeViewItem : ValidatableBindableBase
    {
        private string _name;
        private string _path; 
        private bool _isSelected;
        private bool _isExpanded;
        private ObservableCollection<HuePropertyTreeViewItem> _childrens;

        public HuePropertyTreeViewItem()
        {
            _childrens = new ObservableCollection<HuePropertyTreeViewItem>();
            _isExpanded = false;
            _isSelected = false;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public bool IsSelected
        {
            get => _isSelected && _childrens.Count == 0;
            set => SetProperty(ref _isSelected,value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded,value);
        }

        public string Path
        {
            get => _path;
            set => SetProperty(ref _path,value);
        }

        public ObservableCollection<HuePropertyTreeViewItem> Childrens
        {
            get => _childrens;
            set => SetProperty(ref _childrens,value);
        }
    }
}
