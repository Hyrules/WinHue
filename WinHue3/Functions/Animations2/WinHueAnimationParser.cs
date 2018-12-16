using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sprache;
using WinHue3.ExtensionMethods;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHue3.Functions.Animations2
{
    public static class WinHueAnimationParser
    {
        private static readonly Parser<char> SpaceDelimiter = Parse.WhiteSpace;
        private static readonly Parser<char> LineDelimiter = Parse.Char(';');

        private static readonly Parser<ushort> HueProp =
            from prop in Parse.IgnoreCase("HUE").Text()
            from sep in Parse.Char(':')
            from val in Parse.Number
            select ushort.Parse(val);

        private static readonly Parser<byte> BriProp =
            from prop in Parse.IgnoreCase("BRI").Text()
            from sep in Parse.Char(':')
            from val in Parse.Number
            select byte.Parse(val);

        private static readonly Parser<byte> SatProp =
            from prop in Parse.IgnoreCase("SAT").Text()
            from sep in Parse.Char(':')
            from val in Parse.Number
            select byte.Parse(val);

        private static readonly Parser<ushort> CtProp =
            from prop in Parse.IgnoreCase("CT").Text()
            from sep in Parse.Char(':')
            from val in Parse.Number
            select ushort.Parse(val);

        private static readonly Parser<bool> OnProp =
            from prop in Parse.IgnoreCase("On").Text()
            from sep in Parse.Char(':')
            from val in (Parse.IgnoreCase("TRUE").Or(Parse.IgnoreCase("FALSE"))).Text()
            select bool.Parse(val);

        private static readonly Parser<KeyValuePair<string,int>> WaitCommand =
            from wait in Parse.IgnoreCase("WAIT").Text()
            from sep in Parse.Char(':')
            from value in Parse.Number
            select new KeyValuePair<string, int>(wait, Convert.ToInt32(value));


        private static readonly Parser<Tuple<string, string>> setter =
            from typeword in (Parse.Token(Parse.IgnoreCase("LIGHT")).Or(Parse.Token(Parse.IgnoreCase("GROUP")))).Text()
            from id in Parse.Digit.DelimitedBy(SpaceDelimiter).Text()
            select new Tuple<string, string>(typeword, id);

     /*   private static readonly Parser<KeyValuePair<string, dynamic>> property =
            from prop in BriProp.Token().Or(HueProp.Token())
            select new KeyValuePair<string, dynamic>(prop, value);

        private static readonly Parser<Dictionary<string,dynamic>> properties =
            from first in property
            from rest in Parse.WhiteSpace.Then(_ => property)
            select new Dictionary<string, dynamic>() {  }
            

        private static readonly Parser<IHueObject> SetCommand =
            from set in Parse.Token(Parse.IgnoreCase("SET"))
            from type in setter
            from to in Parse.Token(Parse.IgnoreCase("TO"))
            from state in properties
            select CreateHueObject(type, state);

        private static State CreateState(byte bri, byte sat, ushort ct, bool on)
        {    
            if(bri == null && sat == null && ct == null && on == null) 
            return new State() { bri = bri, sat = sat, ct = ct, on = on };
        }

        private static IHueObject CreateHueObject(Tuple<string, string> type, State properties)
        {
            IHueObject ho = HueObjectCreator.CreateHueObject(type.Item1);
            ho.Id = type.Item2;

            PropertyInfo stateprop = ho.GetType().GetProperty("state");
            stateprop?.SetValue(ho, properties);

            return ho;
        }

        public static object ParseAnimation(string text)
        {
            return hueobject.Parse(text);
        }*/
    }
}