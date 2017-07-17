using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HueLib2.Objects.HueObject
{
    public interface IHueObject : INotifyPropertyChanged
    {
        string Id { get; set; }
        string Name { get; set; }
        ImageSource Image { get; set; }

        object Clone();
    }
}
