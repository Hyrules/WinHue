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
        private static readonly Parser<>
        private static readonly Parser<int> WaitCommand =
            from wait in Parse.IgnoreCase("WAIT")
            from sep in Parse.Char(':')
            from value in Parse.Digit.DelimitedBy(LineDelimiter).Text()
            select Convert.ToInt32(value);

        private static readonly Parser<Tuple<string, string>> setter =
            from typeword in (Parse.Token(Parse.IgnoreCase("LIGHT")).Or(Parse.Token(Parse.IgnoreCase("GROUP")))).Text()
            from id in Parse.Digit.DelimitedBy(SpaceDelimiter).Text()
            select new Tuple<string, string>(typeword, id);

        private static readonly Parser<KeyValuePair<string, string>> property =
            from prop in (
                Parse.IgnoreCase("BRI")
                .Or(Parse.IgnoreCase("SAT")               
                .Or(Parse.IgnoreCase("CT")
                .Or(Parse.IgnoreCase("HUE")
                .Or(Parse.IgnoreCase("ON")
                .Or(Parse.IgnoreCase("TT"))))))).Text()
            from sep in Parse.Char(':')
            from val in Parse.Digit.Many().Text()
            select new KeyValuePair<string, string>(prop, val);

        private static readonly Parser<Dictionary<string, string>> properties =
            from prop in property.DelimitedBy(SpaceDelimiter)
            from lineend in LineDelimiter
            select new Dictionary<string, string>(prop.ToDictionary(x => x.Key,x => x.Value));

        private static readonly Parser<IHueObject> hueobject =
            from set in Parse.Token(Parse.IgnoreCase("SET"))
            from type in setter
            from to in Parse.Token(Parse.IgnoreCase("TO"))
            from props in properties
            select CreateHueObject(type,props);

        private static IHueObject CreateHueObject(Tuple<string,string> type ,Dictionary<string,string> properties)
        {
            IHueObject ho = HueObjectCreator.CreateHueObject(type.Item1);
            ho.Id = type.Item2;
            State newstate = new State();

            PropertyInfo[] pi = newstate.GetType().GetProperties();

            foreach(KeyValuePair<string,string> kvp in properties)
            {
                if (pi.First(x => x.Name.Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase)) != null)
                {
                    int index = pi.FindIndex(x => x.Name.Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase));
                    Type t = Nullable.GetUnderlyingType(pi[index].PropertyType) ?? pi[index].PropertyType;
                    try
                    {
                        dynamic obj = Convert.ChangeType(kvp.Value, t);
                        pi[index]?.SetValue(newstate,obj);
                    }
                    catch (Exception ex)
                    {
                        throw new ParseException($"Invalid value for {kvp.Key} : {kvp.Value}", ex);
                    }

                }
            }

            PropertyInfo stateprop = ho.GetType().GetProperty("state");
            stateprop.SetValue(ho, newstate);

            return ho;
        }

        public static object ParseAnimation(string text)
        {
        
            return hueobject.Parse(text);
        }
    }
}
