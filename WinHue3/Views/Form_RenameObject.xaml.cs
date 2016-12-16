using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using HueLib2;
using MessageBox = System.Windows.MessageBox;

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

            MethodInfo mi = typeof(Bridge).GetMethod("RenameObject");
            MethodInfo generic = mi.MakeGenericMethod(_obj.GetType());
            CommandResult comres = (CommandResult)generic.Invoke(_bridge, new object[] {_obj.Id, tbNewName.Text});

            if (comres.Success)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBoxError.ShowLastErrorMessages(_bridge);
            }
 
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
