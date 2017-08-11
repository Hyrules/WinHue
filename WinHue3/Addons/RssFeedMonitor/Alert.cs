using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace WinHue3.Addons.RssFeedMonitor
{
    [DataContract,Serializable]
    public class Alert
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public ObservableCollection<Criteria> Criterias { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public bool Enabled { get; set; }
    
        [DataMember]
        public List<string> lights { get; set; }

        [DataMember]
        public bool Triggered { get; set; }

        [DataMember]
        public DateTime LastTimeTriggered { get; set; }

    }
}