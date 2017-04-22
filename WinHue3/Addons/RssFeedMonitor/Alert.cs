using System;
using System.Runtime.Serialization;
using System.Windows;
using System.Collections.Generic;
using HueLib2;
using System.Collections.ObjectModel;

namespace WinHue3
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
        public Body Action { get; set; }

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