using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for Form_SceneMapping.xaml
    /// </summary>
    public partial class Form_SceneMapping : Window
    {
        private Bridge _bridge;
        private SceneMappingView _smv;
        public Form_SceneMapping(Bridge br)
        {
            InitializeComponent();
            _bridge = br;
            _smv = new SceneMappingView(br);
            DataContext = _smv;
        }

        private void dgListScenes_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            if (dgListScenes.Columns.Count < 1) return;
            dgListScenes.Columns[0].Visible = false;
        }
    }
}
