namespace HueLib_base
{
    /// <summary>
    /// Sensor for the daylight saving time.
    /// </summary>
    public class DaylightSensorState : SensorState
    {
        /// <summary>
        /// daylight saving time or not.
        /// </summary>
        public bool? daylight { get; set; }

    }
}
