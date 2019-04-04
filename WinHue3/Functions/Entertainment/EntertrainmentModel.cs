using WinHue3.Utils;

namespace WinHue3.Functions.Entertainment
{
    public class EntertrainmentModel : ValidatableBindableBase
    {
        private decimal _x;
        private decimal _y;
        private decimal _z;
        private string _name;
        private string _description;

        public EntertrainmentModel()
        {
            X = 0.00m;
            Y = 0.00m;
            Z = 0.00m;
        }

        public decimal X { get => _x; set => SetProperty(ref _x,value); }
        public decimal Y { get => _y; set => SetProperty(ref _y, value); }
        public decimal Z { get => _z; set => SetProperty(ref _z, value); }

        public string Name { get => _name; set => SetProperty(ref _name,value); }
        public string Description { get => _description; set => SetProperty(ref _description,value); }
    }
}
