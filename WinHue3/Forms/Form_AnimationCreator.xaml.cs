using System;
using System.Collections.Generic;
using System.IO;
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
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.AvalonEdit.Highlighting;
namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_AnimationCreator.xaml
    /// </summary>
    public partial class Form_AnimationCreator : Window
    {
 

        private Bridge _bridge;
        private AnimatorView av;

        public Form_AnimationCreator(Bridge br)
        {
            InitializeComponent();
            av = new AnimatorView(br,ateEditor);
            DataContext = av;
            _bridge = br;

        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            


        }
    }
}
