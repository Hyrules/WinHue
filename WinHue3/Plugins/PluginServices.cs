using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Reflection;
using WinHuePluginModule;
using Action = HueLib_base.Action;
using HueLib_base;
using HueLib;

namespace WinHue3
{
    
    [Export(typeof(IWinhuePluginHost))]
    public class PluginServices : IWinhuePluginHost
    {
        private Bridge _bridge;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public PluginServices([Import("Bridge")] Bridge bridge)
        {
            _bridge = bridge;
        }

        public void SetBridge(Bridge bridge)
        {
            _bridge = bridge;
        }

        public Light GetLight(string id)
        {
            return _bridge.GetLight(id);
        }

        public Group GetGroup(string id)
        {
            return _bridge.GetGroup(id);
        }

        public Sensor GetSensor(string id)
        {
            return _bridge.GetSensor(id);
        }

        public Schedule GetSchedule(string id)
        {
            return _bridge.GetSchedule(id);
        }

        public Dictionary<string,Light> GetLightList()
        {
            return _bridge.GetLightList();
        }

        public Dictionary<string,Group> GetGroupList()
        {
            return _bridge.GetGroupList();
        }

        public Dictionary<string,Schedule> GetSchedulesList()
        {
            return _bridge.GetScheduleList();
        }

        public Dictionary<string,Scene> GetSceneList()
        {
            return _bridge.GetScenesList();
        }

        public Dictionary<string,Sensor> GetSensorList()
        {
            return _bridge.GetSensorList();
        }

        public void WriteToLog(string message)
        {
            log.Info(message);
        }

        /// <summary>
        /// Set the state of a group.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupstate"></param>
        /// <returns></returns>
        public List<int> SetGroupState(string id, Action groupstate)
        {
            MessageCollection mc = _bridge.SetGroupAction(id, groupstate);
            List<int> errorcodes = new List<int>();
            
            if (mc != null)
            if (mc.FailureCount > 0)
            {
                errorcodes.AddRange(mc.OfType<Error>().Select(m => (m).type));
            }
            return errorcodes;
        }

        /// <summary>
        /// Set the state of a light.
        /// </summary>
        /// <param name="id">ID of the light to set state.</param>
        /// <param name="lightstate">The new state of the light.</param>
        /// <returns>A list of error code or an empty list of no error.</returns>
        public List<int> SetLightState(string id, State lightstate)
        {
            MessageCollection mc = _bridge.SetLightState(id, lightstate);
            List<int> errorcodes = new List<int>();
                
            if (mc?.FailureCount > 0)
            {
                errorcodes.AddRange(mc.OfType<Error>().Select(m => (m).type));
            }
            return errorcodes;
        }

    }
}
