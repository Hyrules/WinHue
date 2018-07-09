using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Windows.Media;
using WinHue3.Utils;

namespace WinHue3.Functions.HotKeys.Creator
{
    public class HotKeyCreatorModel : ValidatableBindableBase
    {

        private Key _key;
        private ModifierKeys _modifierKeys;
        private Brush _recordbuttoncolor;
        private string _name;
        private string _description;
        private string _id;
        private string _programpath;

        public Brush RecordButtonColor
        {
            get => _recordbuttoncolor;
            set => SetProperty(ref _recordbuttoncolor,value);
        }

        public ModifierKeys ModifierKeys
        {
            get => _modifierKeys;
            set => SetProperty(ref _modifierKeys,value);
        }

        public Key Key
        {
            get => _key;
            set => SetProperty(ref _key,value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description,value);
        }

        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name,value);
        }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id,value);
        }


        public HotKeyCreatorModel()
        {
            _name = string.Empty;
            _description = string.Empty;
            _id = string.Empty;
            _key = default(Key);
            _modifierKeys = default(ModifierKeys);
            _recordbuttoncolor = new SolidColorBrush() { Color = System.Windows.Media.Color.FromArgb(0,240, 240, 240) };
            _programpath = null;
        }

        public string ProgramPath
        {
            get => _programpath;
            set => SetProperty(ref _programpath, value);
        }

    }
}
