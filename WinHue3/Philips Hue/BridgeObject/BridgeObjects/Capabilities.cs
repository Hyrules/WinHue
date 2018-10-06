using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace WinHue3.Philips_Hue.BridgeObject.BridgeObjects
{
    public class CLights
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class Clip
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class Zll
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class Zgp
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CSensors
    {
        [ReadOnly(true)]
        public int available { get; set; }
        public Clip clip { get; set; }
        public Zll zll { get; set; }
        public Zgp zgp { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CGroups
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class Lightstates
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CScenes
    {
        [ReadOnly(true)]
        public int available { get; set; }
        public Lightstates lightstates { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CSchedules
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CConditions
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CActions
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CRules
    {
        [ReadOnly(true)]
        public int available { get; set; }
        public CConditions conditions { get; set; }
        public CActions actions { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class CResourcelinks
    {
        public int available { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class Streaming
    {
        [ReadOnly(true)]
        public int available { get; set; }
        [ReadOnly(true)]
        public int total { get; set; }
        [ReadOnly(true)]
        public int channels { get; set; }
        public override string ToString()
        {
            return available.ToString();
        }
    }

    public class Timezones
    {
        public List<string> values { get; set; }
        public override string ToString()
        {
            return string.Join(",", values);
        }
    }

    public class Capabilities
    {
        
        public CLights lights { get; set; }
        [ExpandableObject,ReadOnly(true)]
        public CSensors sensors { get; set; }
        public CGroups groups { get; set; }
        [ExpandableObject,ReadOnly(true)]
        public CScenes scenes { get; set; }
        public CSchedules schedules { get; set; }
        [ExpandableObject,ReadOnly(true)]
        public CRules rules { get; set; }
        public CResourcelinks resourcelinks { get; set; }
        [ExpandableObject,ReadOnly(true)]
        public Streaming streaming { get; set; }
        [Browsable(false)]
        public Timezones timezones { get; set; }
    }
}
