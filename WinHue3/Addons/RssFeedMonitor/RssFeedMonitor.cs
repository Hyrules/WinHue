using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.Syndication;
using System.Windows.Threading;
using System.Xml;
using Newtonsoft.Json;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.Common;

namespace WinHue3.Addons.RssFeedMonitor
{
    public class RssFeedMonitor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<Monitor> _monitors;
        private readonly string _path;
        private readonly Bridge _bridge;

        public RssFeedMonitor(Bridge bridge)
        {
            _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "feeds.json";
            _monitors = new List<Monitor>();
            _bridge = bridge;
        }

        public void AddMonitor(Monitor monitor)
        {
            if (_monitors.Contains(monitor)) return;
            _monitors.Add(monitor);
        }

        public void RemoveMonitor(Monitor monitor)
        {
            if (_monitors.Contains(monitor))
                _monitors.Remove(monitor);
        }

        public void RemoveMonitor(string name)
        {
            _monitors.RemoveAll(x => x.Name == name);
        }

        public void LoadMonitorsFromFile()
        {
            try
            {
                string json = File.ReadAllText(_path);
                _monitors = JsonConvert.DeserializeObject<List<Monitor>>(json);
                foreach (var m in _monitors)
                {
                    m.OnConditionMet += M_OnConditionMet;
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occured while loading the monitors");
                log.Error(ex.Message);
            }
        }

        private async void M_OnConditionMet(object sender, ConditionMetEventArgs e)
        {
            await _bridge.SetStateAsyncTask(e.Action.Action, e.Action.Id);
        }

        public void SaveMonitorsToFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_monitors);
                File.WriteAllText(_path,json);
            }
            catch (Exception ex)
            {
                log.Error("An Error occured while saving the monitors");
                log.Error(ex.Message);
            }
        }

        public void StartMonitor(string name = null)
        {

            if (name != null)
            {
                if (_monitors.All(x => x.Name != name)) return;
                {
                    Monitor m = _monitors.Find(x => x.Name == name);
                    m.StartMonitor();
                }
            }
            else
            {
                foreach (Monitor m in _monitors)
                {
                    m.StartMonitor();
                }

            }

        }

        public void StopMonitor(string name = null)
        {
            if (name != null)
            {
                if (_monitors.All(x => x.Name != name)) return;
                {
                    Monitor m = _monitors.Find(x => x.Name == name);
                    m.StopMonitor();
                }
            }
            else
            {
                foreach (Monitor m in _monitors)
                {
                    m.StopMonitor();
                }

            }
        }

    }

    [DataContract]
    public class Monitor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonIgnore] private DispatcherTimer timer;
        private WebClient wc;
        private bool silent;

        [DataMember]
        public string url { get; set; }

        public Monitor()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 300);
            conditions = new List<MonitorCondition>();
            wc = new WebClient();
            wc.DownloadStringCompleted += Wc_DownloadStringCompleted;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Interval
        {
            get => timer.Interval.Seconds;
            set => timer.Interval = new TimeSpan(0, 0, 0, value);
        }

        public bool Running => timer.IsEnabled;

        [DataMember]
        public List<MonitorCondition> conditions { get; set; }

        [DataMember]
        public MonitorAction action { get; set; }


        public bool Silent
        {
            get => silent;
            set => silent = value;
        }

        public void StartMonitor()
        {
            timer.Start();
        }

        public void StopMonitor()
        {
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            wc.DownloadStringAsync(new Uri(url));

        }


        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                log.Error("An error occured while downloading the feed.");
                log.Error(e.Error.Message);
            }
            else
            {
                XmlReader reader = new XmlTextReader(e.Result);
                SyndicationFeed sf = SyndicationFeed.Load(reader);
                if (sf != null)
                {    
                    foreach (var si in sf.Items)
                    {
                        CheckConditions(si);
                    }
                    timer.Start();
                }
                else
                {
                    log.Error($"An error occured while loading the syndication feeds {Name}");
                }

            }
        }

        public void CheckConditions(SyndicationItem si)
        {
            bool conditionmet = false;

            foreach (var c in conditions)
            {
                PropertyInfo pi = si.GetType().GetProperty(c.element);

                switch (c.op)
                {
                    case MonitorCondition.OpType.Greater:
                        
                        
                        break;
                    case MonitorCondition.OpType.Lower:
                        break;
                    case MonitorCondition.OpType.Equal:
                        break;
                    case MonitorCondition.OpType.Contain:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if(conditionmet)
                OnConditionMet?.Invoke(this, new ConditionMetEventArgs(action));
            
        }

      //  public static bool FindConditionGreater()

        public delegate void ConditionMetHandler(object sender, ConditionMetEventArgs e);
        public event ConditionMetHandler OnConditionMet;

    }


    public class MonitorAction
    {
        public string Id { get; set; }
        public HueObjectType Type { get; set; }
        public IBaseProperties Action { get; set; }
    }

    [DataContract]
    public class MonitorCondition
    {
        public enum OpType
        {
            Greater,
            Lower,
            Equal,
            Contain
        };

        [DataMember]
        public string element { get; set; }

        [DataMember]
        public OpType op { get; set; }

        [DataMember]
        public string value { get; set; }
    }

    public class ConditionMetEventArgs : EventArgs
    {
        public MonitorAction Action { get; private set; }

        public ConditionMetEventArgs(MonitorAction action)
        {
            Action = action;
        }
    }


}
