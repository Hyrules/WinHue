using System;
using System.Collections.Generic;
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
using HueLib;
using HueLib_base;

namespace WinHue3
{
  
    
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Form_RenameObject : Window
    {
        private readonly Bridge _bridge;
        private readonly HueObject _obj;
        public Form_RenameObject(Bridge bridge,HueObject obj)
        {

            InitializeComponent();
            _bridge = bridge;
            _obj = obj;
            Title = string.Format(Title,  obj.GetName());
            tbNewName.Text = obj.GetName();

        }

        private void btnRename_Click(object sender, RoutedEventArgs e)
        {
            if (_obj is Light)
            {
                _bridge.ChangeLightName(_obj.Id, tbNewName.Text);
            }
            else if (_obj is Group)
            {
                _bridge.ChangeGroupName(_obj.Id, tbNewName.Text);
            }
            else if (_obj is Schedule)
            {
                _bridge.ChangeScheduleName(_obj.Id, tbNewName.Text);
            }
            else if (_obj is Sensor)
            {
                _bridge.ChangeSensorName(_obj.Id, tbNewName.Text);
            }
            else if (_obj is Rule)
            {
                _bridge.ChangeRuleName(_obj.Id, tbNewName.Text);
            }
            else if(_obj is Scene)
            {
                _bridge.ChangeSceneName(_obj.Id, tbNewName.Text);
            }
            if(_bridge.lastMessages.FailureCount == 0)
                DialogResult = true;
            else
            {
                MessageBox.Show(GlobalStrings.Error_Renaming, GlobalStrings.Error, MessageBoxButton.OK,MessageBoxImage.Error);
                DialogResult = false;
            }
            Close();
        }

        public string GetNewName()
        {
            return tbNewName.Text;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
