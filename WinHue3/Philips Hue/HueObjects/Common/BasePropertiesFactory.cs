using System;
using System.Collections.Generic;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;

namespace WinHue3.Philips_Hue.HueObjects.Common
{

    public static class BasePropertiesCreator
    {
        public static IBaseProperties CreateBaseProperties(string proptype)
        {
            switch (proptype)
            {
                case "group":
                case "groups":
                case "action":
                    return new Action();
                case "lights":
                case "light":
                case "state":
                    return new State();
               /* case "scene":
                    return new Scene();*/
                default:
                    throw new NotSupportedException($"{proptype} not supported.");
            }
        }

        public static IBaseProperties CreateBaseProperties(Type proptype)
        {
            Dictionary<Type, IBaseProperties> def = new Dictionary<Type, IBaseProperties>()
            {
                {typeof(Light), new State() },
                {typeof(Group), new Action()},
                {typeof(State), new State() },
                {typeof(Action),new Action()},
                {typeof(Scene), new Action()}
            };
            //if(proptype != typeof(Light) && proptype != typeof(Group)) throw new NotSupportedException($"{proptype} not supported.");
            return def[proptype];
        }
    }

}
