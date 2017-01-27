using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WinHue3.Models
{
    public class HotKeyCreatorModel : ValidatableBindableBase
    {

        private Key _key;
        private ModifierKeys _modifierKeys;
        private Brush _recordbuttoncolor;
        private string _name;
        private string _description;
        private KeyEventArgs _recordedKeys;
        private bool _isGeneric;

        public Brush RecordButtonColor
        {
            get { return _recordbuttoncolor; }
            set { SetProperty(ref _recordbuttoncolor,value);}
        }

        public ModifierKeys ModifierKeys
        {
            get { return _modifierKeys; }
            set { SetProperty(ref _modifierKeys,value); }
        }

        public Key Key
        {
            get { return _key; }
            set { SetProperty(ref _key,value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description,value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name,value); }
        }

        public bool IsGeneric
        {
            get { return _isGeneric; }
            set { SetProperty(ref _isGeneric,value); }
        }

        public HotKeyCreatorModel()
        {
            RecordButtonColor = new SolidColorBrush() { Color = Color.FromRgb(240, 240, 240) };
        }


    }
}
