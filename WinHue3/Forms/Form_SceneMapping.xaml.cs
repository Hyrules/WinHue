using System;
using System.Windows;
using HueLib2;

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
