using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WinHue3.Functions.Animations
{
    public interface IAnimationStep
    {
        string Name { get; set; }
        string Description { get; set; }
        ImageSource Image { get; set; }
    }
}
