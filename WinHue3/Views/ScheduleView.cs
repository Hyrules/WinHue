using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using HueLib;
using HueLib_base;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows;

namespace WinHue3
{
    /// <summary>
    /// Interface between the bindings of the Schedule Creator view and the Schedule Model.
    /// </summary>
    public class ScheduleView : View
    {
        private Schedule _schedule;
        private string _type;
        private string _smask;
        private string _timeformat;
        private string _randomizetime;
        private string _objtype;
        private string _objid;
        private string _api;

        #region //****************************** CTOR ***********************************
        public ScheduleView(int id,string apikey)
        {
            _schedule = new Schedule {command = new Command {body = new Body()}};
            _timeformat = "yyyy-MM-dd HH:mm:ss";
            _type = "T";
            _smask = "127";
            _randomizetime = string.Empty;
            _schedule.command.body.on = true;
            _schedule.localtime = BuildScheduleLocaltime(DateTime.Now.AddMinutes(5),_type);
            _schedule.command.method = "PUT";
            _objid = id.ToString();
            _api = apikey;
        }

        public ScheduleView(HueObject obj, string apikey)
        {
            _objid = obj.Id;
            _api = apikey;
            if (obj is Schedule)
            {
                _schedule = (Schedule) obj;
                _schedule.time = null;
                _type = GetScheduleTypeFromTime(_schedule.localtime);
                TimeFormatString = _type == "T" ? "yyyy-MM-dd HH:mm:ss" : "HH:mm:ss";
                if (_type == "W")
                {
                    _smask = GetMaskFromAlarm(_schedule.localtime);
                }
                if (_schedule.localtime.Contains("A"))
                {
                    _schedule.localtime = _schedule.localtime.Remove(_schedule.localtime.IndexOf("A"), _schedule.localtime.Length - _schedule.localtime.IndexOf("A"));
                    _smask = "127";
                }
                else
                {
                    _randomizetime = string.Empty;
                    _smask = "127";
                }
                
            }
            else
            {
                _schedule = new Schedule { command = new Command { body = new Body() } };
                _timeformat = "yyyy-MM-dd HH:mm:ss";
                _type = "T";
                _smask = "127";
                _randomizetime = string.Empty;
                _schedule.command.body.on = true;
                _schedule.localtime = BuildScheduleLocaltime(DateTime.Now.AddMinutes(5), _type);
                _schedule.command.method = "PUT";
                _objtype = obj is Light ? "lights" : "groups";
                SetAddress();
            }
            OnPropertyChanged("ScheduleMask");
            OnPropertyChanged("StartTimeText");
        }
        #endregion

        #region //******************************* Properties ****************************

        public Visibility IsAlarm => _type == "W" ? Visibility.Visible : Visibility.Hidden;

        public string StartTimeText
        {
            get
            {
                return _type == "PT" ? Resources.GUI.ScheduleCreatorForm_StartTimeTimer : Resources.GUI.ScheduleCreatorForm_StartTime;
            }

        }

        public double Transitiontime
        {
            get
            {
                return _schedule.command.body.transitiontime ?? -1;
            }

            set
            {
                if (value == -1)
                    _schedule.command.body.transitiontime = null;
                else
                    _schedule.command.body.transitiontime = (int)value;
            }
        }

        public string TimeFormatString
        {
            get { return _timeformat; }
            set
            {
                _timeformat = value;
                OnPropertyChanged("TimeFormatString");
            }
        }

        public string ScheduleMask
        {
            get
            {
                string result = string.Empty;
                byte mask = Convert.ToByte(_smask);

                if ((mask & 64) != 0)
                {
                    result = result + "Mon";
                }

                if((mask & 32) !=0)
                {
                    result = result + "," + "Tue";
                }

                if ((mask & 16) != 0)
                {
                    result = result + "," + "Wed";
                }

                if ((mask & 8) != 0)
                {
                    result = result + "," + "Thu";
                }

                if ((mask & 4) != 0)
                {
                    result = result + "," + "Fri";
                }

                if ((mask & 2) != 0)
                {
                    result = result + "," + "Sat";
                }

                if ((mask & 1) != 0)
                {
                    result = result + "," + "Sun";
                }

                return result;
            }
            set
            {
                byte mask = 0;
                string[] val = value.ToString().Split(',');

                foreach (string s in val)
                {
                    switch (s)
                    {
                        case "Mon":
                            mask = Convert.ToByte(mask + 64);
                            break;
                        case "Tue":
                            mask = Convert.ToByte(mask + 32);
                            break;
                        case "Wed":
                            mask = Convert.ToByte(mask + 16);
                            break;
                        case "Thu":
                            mask = Convert.ToByte(mask + 8);
                            break;
                        case "Fri":
                            mask = Convert.ToByte(mask + 4);
                            break;
                        case "Sat":
                            mask = Convert.ToByte(mask + 2);
                            break;
                        case "Sun":
                            mask = Convert.ToByte(mask + 1);
                            break;
                    }
                }
                
                _smask = $"{mask:D3}";
                if (_smask == "000")
                {
                    SetError(GlobalStrings.Schedule_SelectAtLeastOneDay, "ScheduleMask");
                }
                else
                {
                    RemoveError(GlobalStrings.Schedule_SelectAtLeastOneDay,"ScheduleMask");
                    _schedule.localtime = BuildScheduleLocaltime(Localtime, _type);
                    OnPropertyChanged();
                }
            }
        }

        public int Type
        {
            get
            {             
                switch (_type)
                {
                    case "T":
                        RemoveError("ScheduleMask");
                        return 0;
                    case "PT":
                        RemoveError("ScheduleMask");
                        return 1;
                    case "W":
                        return 2;
                    default:
                        return 0;
                }
            }
            set
            {
                switch (value)
                {
                    case 0:
                        _type = "T";                 
                        break;
                    case 1:
                        _type = "PT";
                        break;
                    case 2:
                        _type = "W";
                        break;
                    default:
                        _type = "T";
                        break;
                }

                TimeFormatString = _type == "T" ? "yyyy-MM-dd HH:mm:ss" : "HH:mm:ss";
                _schedule.localtime = ChangeLocalTimeFormat(_type);
                OnPropertyChanged("IsAlarm");
                OnPropertyChanged();
                OnPropertyChanged("StartTimeText");
            }
        }

        public bool Randomize
        {
            get
            {
                return _randomizetime != string.Empty;
            }
            set
            {
                if (value == false)
                {
                    _randomizetime = string.Empty;
                    if(_schedule.localtime.Contains("A"))
                    {
                        _schedule.localtime = _schedule.localtime.Remove(_schedule.localtime.IndexOf("A"), _schedule.localtime.Length - _schedule.localtime.IndexOf("A"));                      
                    }
                }
                else
                {
                    _randomizetime = "A" + DateTime.Parse("00:" + new Random().Next(1, 59) + ":00").ToString("HH:mm:ss");
                    _schedule.localtime += _randomizetime;
                }
                OnPropertyChanged();
            }

        }

        public bool On
        {
            get { return _schedule.command.body.@on ?? true; }
            set
            {
                _schedule.command.body.on = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _schedule.name; }
            set
            {
                _schedule.name = value;
                OnPropertyChanged();
            }
        }

        public DateTime Localtime
        {
            get
            {
                if (_schedule.localtime == null) return DateTime.Now;

                string actualvalue = _schedule.localtime;
                DateTime dtconversion;

                if (actualvalue.Contains("PT"))
                    actualvalue = actualvalue.Replace("PT", "");
                else if (actualvalue.Contains("W"))
                    actualvalue = actualvalue.Remove(0, 6);
                else if (actualvalue.Contains("T"))
                    actualvalue = actualvalue.Replace("T", " ");

                if (actualvalue.Contains("A"))
                {
                    actualvalue = actualvalue.Remove(actualvalue.IndexOf("A"), actualvalue.Length - actualvalue.IndexOf("A"));
                }

                DateTime.TryParse(actualvalue, out dtconversion);

                if (dtconversion == DateTime.MinValue)
                {
                    dtconversion = DateTime.Now;
                }
                return dtconversion;
            }
            set
            {
                _schedule.localtime = BuildScheduleLocaltime(value, _type);
                OnPropertyChanged();
            }

        }

        public string Description
        {
            get { return _schedule.description; }
            set
            {
                _schedule.description = value;
                OnPropertyChanged();
            }
        }

        public double Hue
        {
            get { return _schedule.command.body.hue ?? -1; }
            set
            {
                if (value == -1)
                    _schedule.command.body.hue = null;
                else
                    _schedule.command.body.hue = Convert.ToInt32(value);
                OnPropertyChanged();
                if (_schedule.command.body.ct != null)
                {
                    _schedule.command.body.ct = null;
                    OnPropertyChanged("Ct");
                }

                if (_schedule.command.body.xy?.x != null)
                {
                    _schedule.command.body.xy = null;
                    OnPropertyChanged("X");
                    OnPropertyChanged("Y");
                }
            }
        }

        public double Sat
        {
            get { return _schedule.command.body.sat ?? -1; }
            set
            {
                if (value == -1)
                    _schedule.command.body.sat = null;
                else
                    _schedule.command.body.sat = Convert.ToByte(value);
                OnPropertyChanged();
            }
        }

        public double Ct
        {
            get { return _schedule.command.body.ct ?? -1; }
            set
            {
                if (value == 152)
                {
                    _schedule.command.body.ct = null;
                }
                else
                {
                    _schedule.command.body.ct = (ushort)value;
                }
                OnPropertyChanged();
                if (_schedule.command.body.hue != null)
                {
                    _schedule.command.body.hue = null;
                    OnPropertyChanged("Hue");
                }

                if (_schedule.command.body.xy?.x != null)
                {
                    _schedule.command.body.xy = null;
                    OnPropertyChanged("X");
                    OnPropertyChanged("Y");
                }
            }
        }

        public double Bri
        {
            get { return _schedule.command.body.bri ?? -1; }
            set
            {
                if (value == -1)
                {
                    _schedule.command.body.bri = null;
                }
                else
                    _schedule.command.body.bri = Convert.ToByte(value);
                OnPropertyChanged();
            }
        }

        public bool Enabled
        {
            get
            {
                return _schedule.status == null || _schedule.status.Equals("enabled");
            }
            set
            {
                _schedule.status = (value == true ? "enabled" : "disabled");
                OnPropertyChanged();
            }
        }

        public bool? Autodelete
        {
            get
            {
                return _schedule.command.body.autodelete;
            }

            set
            {
                _schedule.command.body.autodelete = value;
                OnPropertyChanged();
            }
        }

        public string Scene
        {
            get
            {
                return _schedule.command.body.scene;
            }

            set
            {
                _schedule.command.body.scene = value;
                OnPropertyChanged();
            }
        }

        public decimal X
        {
            get
            {
                if (_schedule.command.body.xy?.x == null)
                    return (decimal)-0.001;
                return (decimal)_schedule.command.body.xy.x;
            }

            set
            {
                if (_schedule.command.body.xy == null)
                    _schedule.command.body.xy = new XY();
                if (value == (decimal)-0.001)
                {
                    _schedule.command.body.xy = null;
                    OnPropertyChanged("Y");
                }
                else
                {
                    _schedule.command.body.xy.x = value;
                    if (_schedule.command.body.xy?.y == null)
                    {
                        _schedule.command.body.xy.y = 0;
                        OnPropertyChanged("Y");
                    }
                    if (_schedule.command.body.ct != null)
                    {
                        _schedule.command.body.ct = null;
                        OnPropertyChanged("Ct");
                    }

                    if (_schedule.command.body.hue != null)
                    {
                        _schedule.command.body.hue = null;
                        OnPropertyChanged("Hue");
                    }
                }


                OnPropertyChanged();
            }
        }

        public decimal Y
        {
            get
            {
                if (_schedule.command.body.xy?.y == null)
                    return -0.001M;
                return (decimal)_schedule.command.body.xy.y;
            }

            set
            {
                if (_schedule.command.body.xy == null)
                    _schedule.command.body.xy = new XY();
                if (value == -0.001M)
                {
                    _schedule.command.body.xy = null;
                    OnPropertyChanged("X");
                }
                else
                {
                    _schedule.command.body.xy.y = value;
                    if (_schedule.command.body.xy?.x == null)
                    {
                        _schedule.command.body.xy.x = 0;
                        OnPropertyChanged("X");
                    }

                    if (_schedule.command.body.ct != null)
                    {
                        _schedule.command.body.ct = null;
                        OnPropertyChanged("Ct");
                    }

                    if (_schedule.command.body.hue != null)
                    {
                        _schedule.command.body.hue = null;
                        OnPropertyChanged("Hue");
                    }
                }


                OnPropertyChanged();
            }
        }
        #endregion

        #region //******************************* Methods *******************************

        public void SetAddress()
        {
            string objstruct = _objtype == "groups" ? "action" : "state";
            _schedule.command.address = $@"/api/{_api}/{_objtype}/{_objid}/{objstruct}";
        }

        private string ChangeLocalTimeFormat(string type)
        {
            
            DateTime dt = Localtime;
            return BuildScheduleLocaltime(dt, type);
        }

        public Schedule GetSchedule()
        {
            return _schedule;
        }

        private string BuildScheduleLocaltime(DateTime timevalue,string type)
        {
            string time = timevalue.ToString("yyyy-MM-dd HH:mm:ss");
            switch (type)
            {
                case "T":
                    time = time.Replace(" ", "T");
                    break;
                case "PT":
                    time = time.Split(' ')[1];
                    time = time.Insert(0, "PT");
                    break;
                case "W":
                    time = time.Split(' ')[1];
                    time = time.Insert(0, "W" + _smask + "/T");
                    break;
                default:
                    time = time.Replace(" ", "T");
                    break;
            }
            time += _randomizetime;
            return time;
        }

        private string GetMaskFromAlarm(string timestring)
        {
            return timestring.Substring(1, 3);
        }

        private string GetScheduleTypeFromTime(string timestring)
        {
            if (timestring.Contains("PT")) return "PT";
            if (timestring.Contains("W")) return "W";   
            return "T";
        }

        #endregion

    }


}
