using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.ResourceLinkObject;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;

namespace WinHue3.Philips_Hue.HueObjects.Common
{
    public static class HueObjectCreator 
    {
        public static IHueObject CreateHueObject(string huetype)
        {
            switch (huetype)
            {
                case "LIGHT":
                case "lights":
                case "light":
                    return new Light();
                case "GROUP":
                case "groups":
                case "group":
                    return new Group();
                case "SCHEDULE":
                case "schedules":
                case "schedule":
                    return new Schedule();
                case "RULE":
                case "rules":
                case "rule":
                    return new Rule();
                case "RESOURCELINK":
                case "resourcelinks":
                case "resourcelink":
                    return new Resourcelink();
                case "SENSOR":
                case "sensors":
                case "sensor":
                    return new Sensor();
                case "SCENE":
                case "scenes":
                case "scene":
                    return new Scene();
                default:
                    return null;
            }
        }
    }
}
