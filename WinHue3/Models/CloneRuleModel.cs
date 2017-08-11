using IHueObject = WinHue3.Philips_Hue.HueObjects.Common.IHueObject;

namespace WinHue3.Models
{
    public class CloneRuleModel
    {
        public IHueObject replacingobject { get; set; }
        public IHueObject withobject { get; set; }

        public override string ToString()
        {
            return $"Replacing {replacingobject.Id} with {withobject.Id}";
        }
    }
}
