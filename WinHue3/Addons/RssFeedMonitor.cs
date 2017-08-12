using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;
using WinHue3.Philips_Hue.HueObjects.Common;
using System.ServiceModel.Syndication;
using System.Xml;

namespace WinHue3.Addons
{
    public class RssFeedMonitor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<Monitor> monitors;
        private readonly string path;

        public RssFeedMonitor()
        {
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "feeds.json";
            monitors = new List<Monitor>();
        }

        public void AddMonitor(Monitor monitor)
        {
            if (monitors.Contains(monitor)) return;
            monitors.Add(monitor);
        }

        public void RemoveMonitor(Monitor monitor)
        {
            if (monitors.Contains(monitor))
                monitors.Remove(monitor);
        }

        public void RemoveMonitor(string name)
        {
            monitors.RemoveAll(x => x.Name == name);
        }

        public void LoadMonitorsFromFile()
        {
            try
            {
                string json = File.ReadAllText(path);
                monitors = JsonConvert.DeserializeObject<List<Monitor>>(json);
            }
            catch (Exception ex)
            {
                log.Error("An error occured while loading the monitors");
                log.Error(ex.Message);
            }
        }

        public void SaveMonitorsToFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(monitors);
                File.WriteAllText(path,json);
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
                if (monitors.All(x => x.Name != name)) return;
                {
                    Monitor m = monitors.Find(x => x.Name == name);
                    m.StartMonitor();
                }
            }
            else
            {
                foreach (Monitor m in monitors)
                {
                    m.StartMonitor();
                }

            }

        }

        public void StopMonitor(string name = null)
        {
            if (name != null)
            {
                if (monitors.All(x => x.Name != name)) return;
                {
                    Monitor m = monitors.Find(x => x.Name == name);
                    m.StopMonitor();
                }
            }
            else
            {
                foreach (Monitor m in monitors)
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

        [JsonIgnore]
        private DispatcherTimer timer;
        private WebClient wc;
        private bool silent;

        [DataMember]
        public string url { get; set; }

        public Monitor()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0,0,0,300);
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
            set => timer.Interval = new TimeSpan(0,0,0,value);
        }

        public bool Running => timer.IsEnabled;

        [DataMember]
        public List<MonitorCondition> conditions { get; set; }

        [DataMember]
        public IBaseProperties action { get; set; }


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
                
            }
            else
            {
                XmlReader reader = new XmlTextReader(e.Result);
                SyndicationFeed sf = SyndicationFeed.Load(reader);
                if (sf != null)
                {
                    foreach (var si in sf.Items)
                    {
                        //TODO : Check conditions are met
                        if (!silent)
                        {
                            //TODO : do action    
                        }

                    }

                    timer.Start();
                }
                else
                {
                    log.Error($"An error occured while loading the syndication feeds {Name}");
                }

            }
        }

        
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


    
}
