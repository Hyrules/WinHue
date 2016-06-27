using System;
using System.Runtime.Serialization;

namespace RssFeedMonitor
{
    [DataContract,Serializable]
    public class Criteria
    {
        [DataMember]
        public string RSSElement { get; set; }
        [DataMember]
        public string Condition { get; set; }
        [DataMember]
        public string UserCondition { get; set; }

        public override string ToString()
        {
            return $"{RSSElement} {Condition} {UserCondition}";
        }
    }
}