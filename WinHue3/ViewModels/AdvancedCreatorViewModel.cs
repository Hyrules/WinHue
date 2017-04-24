using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WinHue3.ViewModels
{
    public class AdvancedCreatorViewModel : ValidatableBindableBase
    {
        public AdvancedCreatorViewModel()
        {
        }

        private void CreateScheduleTemplate()
        {

        }

        private void CreateSensorTemplate()
        {

        }

        private void CreateRuleTemplate()
        {

        }

        private void CreateRessourceLinkTemplate()
        {

        }

        private void CreateGroupTemplate()
        {

        }

        public ICommand CreateScheduleTemplateCommand => new RelayCommand(param => CreateScheduleTemplate());
        public ICommand CreateSensorTemplateCommand => new RelayCommand(param => CreateSensorTemplate());
        public ICommand CreateRuleTemplateCommand => new RelayCommand(param => CreateRuleTemplate());
        public ICommand CreateRessourceLinkTemplateCommand => new RelayCommand(param => CreateRessourceLinkTemplate());
        public ICommand CreateGroupTemplateCommand => new RelayCommand(param => CreateGroupTemplate());
    }
}