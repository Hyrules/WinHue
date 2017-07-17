using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WinHue3.Models.AppSettings
{
    public class AppViewSettingsModel : ValidatableBindableBase
    {
        private WinHueSortOrder _sort;
        private bool _showId;
        private bool _wrap;

        public AppViewSettingsModel()
        {
            Sort = WinHueSortOrder.Default;
            ShowId = false;
            Wrap = false;
        }


        public WinHueSortOrder Sort
        {
            get { return _sort; }
            set { SetProperty(ref _sort,value); }
        }

        public bool ShowId
        {
            get { return _showId; }
            set { SetProperty(ref _showId,value); }
        }

        public bool Wrap
        {
            get { return _wrap; }
            set { SetProperty(ref _wrap,value); }
        }
    }
}
