namespace WinHue3.Philips_Hue.HueObjects.Common
{

    public interface IBaseProperties
    {

        
        bool? on { get; set; }

        byte? bri { get; set; }
  
        ushort? hue { get; set; }

        byte? sat { get; set; }

        decimal[] xy { get; set; }

        string effect { get; set; }

        string colormode { get; set; }

        ushort? transitiontime { get; set; }

        string alert { get; set; }

        short? bri_inc { get; set; }

        short? sat_inc { get; set; }

        ushort? ct { get; set; }

        int? hue_inc { get; set; }

        short? ct_inc { get; set; }

        decimal[] xy_inc { get; set; }

        string ToString();


    }

}
