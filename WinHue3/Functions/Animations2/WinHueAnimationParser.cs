using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Sprache;
using WinHue3.Philips_Hue.HueObjects.Common;
using WinHue3.Philips_Hue.HueObjects.LightObject;

namespace WinHue3.Functions.Animations2
{
    public static class WinHueAnimationParser
    {
        private static readonly Parser<char> SpaceDelimiter = Parse.WhiteSpace;
        private static readonly Parser<char> LineDelimiter = Parse.Char(';');

        private static readonly Parser<int> WaitCommand =
            from wait in Parse.String("WAIT ")
            from value in Parse.Digit.DelimitedBy(LineDelimiter).Text()
            select Convert.ToInt32(value);
        
        private static readonly Parser<Tuple<string, string>> type =
            from typeletter in Parse.Letter.Once().Text()
            from id in Parse.Digit.DelimitedBy(SpaceDelimiter).Text()
            select new Tuple<string,string>(typeletter,id);

        private static readonly Parser<KeyValuePair<string,string>> property =
            from prop in Parse.Letter.Many().Text()
            from sep in Parse.Char(':')
            from val in Parse.Digit.Many().Text()
            select new KeyValuePair<string, string>(prop,val);

        /*private static readonly Parser<Dictionary<string, string>> properties =
            from prop in property.DelimitedBy(SpaceDelimiter).AtLeastOnce().DelimitedBy(LineDelimiter)
            from lineend in LineDelimiter
            select new Dictionary<string, string>(prop.ToDictionary(p => p));*/

        public static object ParseAnimation(string text)
        {
            return new object();
            //return properties.Parse(text);
        }
    }
}
