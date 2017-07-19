using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HueLib2;
using HueLib2.BridgeMessages;
using HueLib2.Objects.HueObject;
using WinHue3.Models;
using WinHue3.Utils;
using WinHue3.ViewModels;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_ResourceLinksCreator.xaml
    /// </summary>
    public partial class Form_ResourceLinksCreator : Window
    {
        private ResourceLinkCreatorViewModel rlcvm;
        private readonly Bridge _bridge;
        private string id;

        public Form_ResourceLinksCreator(Bridge bridge,Resourcelink rl = null)
        {
            _bridge = bridge;
            InitializeComponent();
            rlcvm = this.DataContext as ResourceLinkCreatorViewModel;
            rlcvm.LinkCreatorModel.ShowID = WinHueSettings.settings.ShowID;
            rlcvm.LinkCreatorModel.Wrap = WinHueSettings.settings.WrapText;

            List<IHueObject> hr = HueObjectHelper.GetBridgeDataStore(_bridge);
            if (hr == null) return;
            ObservableCollection<IHueObject> _listbrobj = new ObservableCollection<IHueObject>();
            List<IHueObject> listobj = hr;

            switch (WinHueSettings.settings.Sort)
            {
                case WinHueSortOrder.Default:
                    _listbrobj = new ObservableCollection<IHueObject>(hr);                     
                    break;
                case WinHueSortOrder.Ascending:
                    _listbrobj.AddRange(from item in listobj where item is Light orderby item.Name ascending select item);
                    _listbrobj.AddRange(from item in listobj where item is Group orderby item.Name ascending select item);
                    _listbrobj.AddRange(from item in listobj where item is Schedule orderby item.Name ascending select item);
                    _listbrobj.AddRange(from item in listobj where item is Scene orderby item.Name ascending select item);
                    _listbrobj.AddRange(from item in listobj where item is Sensor orderby item.Name ascending select item);
                    _listbrobj.AddRange(from item in listobj where item is Rule orderby item.Name ascending select item);
                    _listbrobj.AddRange(from item in listobj where item is Resourcelink orderby item.Name ascending select item);
                    break;
                case WinHueSortOrder.Descending:
                    _listbrobj.AddRange(from item in listobj where item is Light orderby item.Name descending select item);
                    _listbrobj.AddRange(from item in listobj where item is Group orderby item.Name descending select item);
                    _listbrobj.AddRange(from item in listobj where item is Schedule orderby item.Name descending select item);
                    _listbrobj.AddRange(from item in listobj where item is Scene orderby item.Name descending select item);
                    _listbrobj.AddRange(from item in listobj where item is Sensor orderby item.Name descending select item);
                    _listbrobj.AddRange(from item in listobj where item is Rule orderby item.Name descending select item);
                    _listbrobj.AddRange(from item in listobj where item is Resourcelink orderby item.Name descending select item);
                    break;
                default:
                    goto case WinHueSortOrder.Default;
            }
            rlcvm.ListHueObjects = _listbrobj;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(rlcvm.ListHueObjects);
            view.GroupDescriptions?.Clear();
            PropertyGroupDescription groupDesc = new TypeGroupDescription();
            view.GroupDescriptions?.Add(groupDesc);

            CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(rlcvm.LinkCreatorModel.ListlinkObject);
            view2.GroupDescriptions?.Clear();
            PropertyGroupDescription groupDesc2 = new TypeGroupDescription();
            view2.GroupDescriptions?.Add(groupDesc2);
            if(rl != null)
                rlcvm.Resourcelink = rl;
        }

        private void btnCreateResourceLink_Click(object sender, RoutedEventArgs e)
        {
            Resourcelink rl = rlcvm.Resourcelink;
            CommandResult<Messages> cr = rlcvm.IsEditing ? _bridge.ModifyObject<Resourcelink>(rl,rl.Id) : _bridge.CreateObject<Resourcelink>(rl);
            
            if (cr.Success)
            {
                if (rlcvm.IsEditing)
                    id = rl.Id;
                else
                    id = cr.Data.SuccessMessages[0].value;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }

        }

        public string GetCreatedModifiedId()
        {
            return id;
        }
        

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
