using System.Windows;
using HueLib2;

namespace WinHue3
{
    /// <summary>
    /// Interaction logic for Form_AnimationCreator.xaml
    /// </summary>
    public partial class Form_AnimationCreator : Window
    {
 

        private Bridge _bridge;
     //   private AnimatorView av;

        public Form_AnimationCreator(Bridge br)
        {
            InitializeComponent();
         //   av = new AnimatorView(br,ateEditor);
        //    DataContext = av;
            _bridge = br;

        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            


        }
    }
}
