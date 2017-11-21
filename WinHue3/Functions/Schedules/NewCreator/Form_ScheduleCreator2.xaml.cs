using System.Threading.Tasks;
using System.Windows;

namespace WinHue3.Functions.Schedules.NewCreator
{
    /// <summary>
    /// Interaction logic for Form_ScheduleCreator2.xaml
    /// </summary>
    public partial class Form_ScheduleCreator2 : Window
    {
        public ScheduleCreatorViewModel _scvm;

        public Form_ScheduleCreator2()
        {
            InitializeComponent();
            _scvm = DataContext as ScheduleCreatorViewModel;
            
        }

        public async Task Initialize()
        {
            await _scvm.Initialize();
        }
    }
}
