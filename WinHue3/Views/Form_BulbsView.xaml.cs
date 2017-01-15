using System.Collections.Generic;
using System.Windows;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_BulbsView.xaml
    /// </summary>
    public partial class Form_BulbsView : Window
    {
        private BulbsViewView _bvv;
        private readonly Bridge _bridge;
        public Form_BulbsView(Bridge bridge)
        {
            InitializeComponent();
            CommandResult lresult = _bridge.GetListObjects<Light>();
            if (!lresult.Success) return;
            _bvv = new BulbsViewView((Dictionary<string, Light>) lresult.resultobject);
            DataContext = _bvv;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_bvv == null)
                Close();
        }
    }
}
