using System.Collections.Generic;
using System.Runtime.InteropServices;
using HueLib2;
using System.Drawing;
using System.Windows.Media;


namespace WinHuePluginModule
{
    /// <summary>
    /// Interface for plugins modules.
    /// </summary>
    [ComImport]
    [Guid("3ACC0580-F21F-4F38-BA92-DD5AE6B3A932")]
    public interface IWinHuePluginModule
    {
        /// <summary>
        /// Show the settings form of the plugin.
        /// </summary>
        /// <returns>True or false or null</returns>
        bool? ShowSettingsForm();
        
        /// <summary>
        /// Start the plugin.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the plugin.
        /// </summary>
        void Stop();

        /// <summary>
        /// Icon of the plugin 64x64.
        /// </summary>
        Bitmap pluginIcon { get; }

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        string pluginName { get; }

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        string pluginDesc { get; }

        /// <summary>
        /// Author of the plugin.
        /// </summary>
        string pluginAuth { get; }
    }

    /// <summary>
    /// Interface for host services.
    /// </summary>
    [ComImport]
    [Guid("3E2D65A5-892E-4CD9-A159-1F5E6239A4D9")]
    public interface IWinhuePluginHost
    {
        /// <summary>
        /// Get the requested light information.
        /// </summary>
        /// <param name="id">ID of the requested light.</param>
        /// <returns>The complete information of a light.</returns>
        Light GetLight(string id);

        /// <summary>
        /// Get the requested group information.
        /// </summary>
        /// <param name="id">ID of the requested group.</param>
        /// <returns>The complete information of a group.</returns>
        Group GetGroup(string id);

        /// <summary>
        /// Get the requested sensor information.
        /// </summary>
        /// <param name="id">ID of the requested sensor.</param>
        /// <returns>The complete information of the sensor.</returns>
        Sensor GetSensor(string id);

        /// <summary>
        /// Get the requested schedule information.
        /// </summary>
        /// <param name="id">ID of the requested schedule.</param>
        /// <returns>The complete information of the schedule.</returns>
        Schedule GetSchedule(string id);

        /// <summary>
        /// Get the list of lights available on the bridge.
        /// </summary>
        /// <returns>A list of light available on the bridge.</returns>
        Dictionary<string, Light> GetLightList();

        /// <summary>
        /// Get the list of groups available on the bridge.
        /// </summary>
        /// <returns>A list of groups available on the bridge.</returns>
        Dictionary<string, Group> GetGroupList();

        /// <summary>
        /// Get the list of schedules available on the bridge.
        /// </summary>
        /// <returns>A list of schedules available on the bridge.</returns>
        Dictionary<string, Schedule> GetSchedulesList();

        /// <summary>
        /// Get the list of scenes available on the bridge.
        /// </summary>
        /// <returns>A list of scenes available on the bridge.</returns>
        Dictionary<string, Scene> GetSceneList();

        /// <summary>
        /// Get the list of sensor available on the bridge.
        /// </summary>
        /// <returns>A list of sensors available on the bridge.</returns>
        Dictionary<string, Sensor> GetSensorList();
        
        /// <summary>
        /// Set the state of a light.
        /// </summary>
        /// <param name="id">ID of the selected light.</param>
        /// <param name="lightstate">The new state of the light.</param>
        /// <returns>A list of int errors or empty if no error.</returns>
        List<int> SetLightState(string id, State lightstate);

        /// <summary>
        /// Set the state of a group.
        /// </summary>
        /// <param name="id">ID of the selected group.</param>
        /// <param name="groupstate">The new state of the group.</param>
        /// <returns>A list of int errors or empty if no error.</returns>
        List<int> SetGroupState(string id, Action groupstate);

        /// <summary>
        /// Write to the event log.
        /// </summary>
        /// <param name="message"></param>
        void WriteToLog(string message);

    }

}

