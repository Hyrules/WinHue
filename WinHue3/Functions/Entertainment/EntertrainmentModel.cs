using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHue3.Utils;

namespace WinHue3.Functions.Entertainment
{
    public class EntertrainmentModel : ValidatableBindableBase
    {
        private decimal _x;
        private decimal _y;
        private decimal _z;

        public EntertrainmentModel()
        {
            X = 0.00m;
            Y = 0.00m;
            Z = 0.00m;
        }

        public decimal X { get => _x; set => SetProperty(ref _x,value); }
        public decimal Y { get => _y; set => SetProperty(ref _y, value); }
        public decimal Z { get => _z; set => SetProperty(ref _z, value); }
    }
}
