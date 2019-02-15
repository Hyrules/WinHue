using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NDesk.Options;
using WinHue3.Functions.Application_Settings.Settings;
using WinHue3.Philips_Hue;
using WinHue3.Philips_Hue.BridgeObject;
using WinHue3.Philips_Hue.HueObjects.GroupObject;
using WinHue3.Philips_Hue.HueObjects.LightObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipGenericStatus;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipHumidity;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipOpenClose;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipPresence;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.ClipZllTemperature;
using WinHue3.Philips_Hue.HueObjects.NewSensorsObject.CLIPGenericFlag;
using WinHue3.Philips_Hue.HueObjects.RuleObject;
using WinHue3.Philips_Hue.HueObjects.SceneObject;
using WinHue3.Philips_Hue.HueObjects.ScheduleObject;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;


namespace whc
{
    public static class ConsoleHandler
    {
  
        static Bridge _bridge;
        internal enum Command { Light, Group, Scene, Schedule, Bridge, None, Creategroup, Deletegroup, Sensor }
        internal static Command Cmd;
        static readonly OptionSet MainOpts;
        static readonly OptionSet ObjOpts;
        static readonly OptionSet LightOpts;
        static readonly OptionSet GroupOpts;
        static readonly OptionSet CreateGrOpts;
        static readonly OptionSet SensorOpts;
        private static ISensorStateBase _sensorstate;
        static Group _grp;
        static bool _error;
        static State _state;
        static Action _action;
        static string _id;
        static bool _noprompt;
        static bool _nomsg;
  

        static ConsoleHandler()
        {
      
            _noprompt = false;
            _nomsg = false;
            Cmd = Command.None;
            #region MAIN_OPTIONS           
            MainOpts = new OptionSet
            {
                {"noprompt", "Set this option if you want to ignore prompts. (This will assume yes for all questions)", delegate(string v){_noprompt = true;}},
                {"nomsg","Cancel the output of messages from the bridge.(The application will not output anything) ", delegate(string v){_nomsg = true;}}
            };
            #endregion
            #region OBJECT_OPTIONS
            ObjOpts = new OptionSet
            {
                {"l|light=","Set the mode to light.", delegate(string v) 
                { 
                    if(Cmd == Command.None)
                    {
                        Cmd = Command.Light;
                        _error = !CheckIdType(v);
                        _id = v;
                        _state = new State();
                    }
                    else
                        _error = true;
                }},
                {"g|group=","Set the mode to group.", delegate(string v) 
                { 
                    if(Cmd == Command.None)
                    {
                        _action = new Action();
                        _error = !CheckIdType(v);
                        _id = v;
                        Cmd = Command.Group;
                    }
                    else
                        _error = true;
                }},             
  /*              {"sc|schedule=","Set the mode to schedule.", delegate(string v)
                { 
                    if(cmd == Command.NONE)
                    {
                        id = v;
                        cmd = Command.SCHEDULE;
                    }
                    else
                        error = true;
                }},*/
                {"sn|scene=","Set the mode to scene.", delegate(string v) 
                { 
                    if(Cmd == Command.None)
                    {
                        _id = v;
                        
                        Cmd = Command.Scene;
                    }
                    else
                        _error = true;
                }},
                {"ss|sensor=","Set the mode to sensor.", delegate(string v)
                {
                    if(Cmd == Command.None)
                    {
                        _id = v;
                        _error = !CheckIdType(v);
                        Cmd = Command.Sensor;
                    }
                    else
                        _error = true;
                }},
                {"cg|creategroup","Create a new group.", delegate(string v)
                {
                    _grp = new Group();
                    if (Cmd == Command.None)
                        Cmd = Command.Creategroup;
                    else
                        _error = true;
                }},
                {"dg|deletegroup=","Delete the selected group", delegate(string v)
                {
                    if (Cmd == Command.None)
                    {
                        Cmd = Command.Deletegroup;
                        if(PromptYesNo() || _noprompt)
                        {
                            bool bresult = _bridge.RemoveObject(new Group(){Id = v});
                            if (bresult)
                            {

                                WriteMessageToConsole($"Group {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting group :" + v);
                        }
                    }
                    else
                        _error = true;                   
                }},
                {"dc|deleteschedule=","Delete the selected schedule", delegate(string v)
                {
                    if (Cmd == Command.None)
                    {
                        if(PromptYesNo() || _noprompt)
                        {
                            bool bresult = _bridge.RemoveObject(new Schedule(){Id = v});
                            if (bresult)
                            {
                                WriteMessageToConsole($"Schedule {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting schedule :" + v);
                        }
                    }
                    else
                        _error = true;                   
                }},
                {"dl|deletelight=","Delete the selected light", delegate(string v)
                {
                    if (Cmd == Command.None)
                    {
                        if(PromptYesNo() || _noprompt)
                        {
                            bool bresult = _bridge.RemoveObject(new Light(){Id =v});         
                            if (bresult)
                            {
                                WriteMessageToConsole($"Light {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting light :" + v);
                        }
                    }
                    else
                        _error = true;                   
                }},
                {"do|deletesensor=","Delete the selected sensor", delegate(string v)
                {
                    if (Cmd == Command.None)
                    {
                        if(PromptYesNo() || _noprompt)
                        {
                            bool bresult = _bridge.RemoveObject(_id,HueObjectType.sensors);
                            if (bresult)
                            {
                                WriteMessageToConsole($"Sensor {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting sensor :" + v);
                        }
                    }
                    else
                        _error = true;                   
                }},
                {"dr|deleterule=","Delete the selected rule", delegate(string v)
                {
                    if (Cmd == Command.None)
                    {
                        if(PromptYesNo() || _noprompt)
                        {
                            bool bresult = _bridge.RemoveObject(new Rule() {Id = v});
                            if (bresult)
                            {
                                WriteMessageToConsole($"Rule {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting rule :" + v);
                        }
                    }
                    else
                        _error = true;                   
                }},
                {"ll", "List the lights available in the bridge", delegate(string v)
                {
                    List<Light> bresult = _bridge.GetListObjects<Light>();
                    if(bresult != null)
                    {
                        List<Light> listLights = bresult;
                        foreach(Light kvp in listLights)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Id}, Name={kvp.name}, ModelID={kvp.modelid}, Type={kvp.type}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of lights :" + bresult );
                    }
                }},
                {"lg", "List the groups available in the bridge", delegate(string v)
                {
                    List<Group> bresult = _bridge.GetListObjects<Group>();
                    if(bresult != null)
                    {
                        List<Group> listgroups = bresult;
                        foreach(Group kvp in listgroups)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Id}, Name={kvp.name}, MembersID=[{string.Join(",", kvp.lights)}], Type={kvp.type}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of groups : " + bresult);
                    }
                }},
                {"ls", "List the scenes available in the bridge", delegate(string v)
                {
                    List<Scene> bresult = _bridge.GetListObjects<Scene>();
                    if(bresult != null)
                    {
                        List<Scene> listscenes = bresult;
                        foreach(Scene kvp in listscenes)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Id}, Name={kvp.name}, MembersID=[{string.Join(",", kvp.lights)}]");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of scenes : " + bresult);
                    }
                }},
                {"lc", "List the schedules available in the bridge", delegate(string v)
                {
                    List<Schedule> bresult = _bridge.GetListObjects<Schedule>();
                    if(bresult != null)
                    {
                        foreach(Schedule kvp in bresult)
                        {
                            WriteMessageToConsole($"[ID]={kvp.Id}, Name={kvp.name}, Time={kvp.localtime}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of schedules : " + bresult);
                    }
                }},
                {"lo", "List the sensors available in the bridge", delegate(string v)
                {
                    List<Sensor> bresult = _bridge.GetListObjects<Sensor>();
                    
                    if(bresult != null)
                    {
                        
                        foreach(Sensor kvp in bresult)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Id}, Name={kvp.name}, Type={kvp.type}, Model={kvp.modelid}, SwVersion={kvp.swversion}, Manufacturer={kvp.manufacturername}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of sensors " + v);
                    }
                }},
                {"lr", "List the rules available in the bridge", delegate(string v)
                {
                    List<Rule> bresult = _bridge.GetListObjects<Rule>();

                    if(bresult != null)
                    {
                        List<Rule> listrules = bresult;
                        foreach(Rule kvp in listrules)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Id}, Name={kvp.name}, Owner={kvp.owner}, Status={kvp.status}, TimesTriggered={kvp.timestriggered}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of rules : " + bresult);
                    }
                }},
                {"lightstate=","Get the state of a light", delegate(string v)
                {
                    byte value = 0;
                    bool isValidid = byte.TryParse(v,out value);
                    
                    if (isValidid && v != "0")
                    {
                        Light bresult = _bridge.GetObject<Light>(v);
                        if (bresult != null)
                        {
                            Light light = bresult;
                            WriteMessageToConsole(light.state.ToString() ?? _bridge.LastCommandMessages.ToString());

                        }
                        else
                        {
                            WriteMessageToConsole("Error getting the light : " + bresult);
                        }
                    }
                    else
                    {
                        WriteMessageToConsole($"ERROR: The provided value {v} is not a valid id for the light. Please use a number between 1 and 255");
                    }
                }},
                {"groupstate=","Get the state of a group", delegate(string v)
                {
                    byte value = 0;
                    bool isValidid = byte.TryParse(v,out value);

                    if (isValidid)
                    {
                        Group bresult = _bridge.GetObject<Group>(v);
                        if (bresult != null)
                        {
                            Group group = bresult;
                            WriteMessageToConsole(group.action.ToString());

                        }
                        else
                        {
                            WriteMessageToConsole("Error getting group : " + bresult);
                        }
                    }
                    else
                    {
                        WriteMessageToConsole($"ERROR: The provided value {v} is not a valid id for the light. Please use a number between 1 and 255");
                    }

                }}

            };
            #endregion
            #region LIGHT_OPTIONS
            LightOpts = new OptionSet
            {
                {"bri=","Brightness of a light [0-254]", delegate(string v)
                {
                    byte bri;
                    if (byte.TryParse(v, out bri))
                        _state.bri = bri;
                    else
                    {
                        WriteMessageToConsole("Error BRI invalid value " + v);
                        _error = true;
                    }
                }},
                {"bri_inc=","Brightness incrementor [-254-254]", delegate(string v)
                {
                    if (short.TryParse(v, out var briInc))
                    {
                        if (briInc >= -254 && briInc <= 254)
                        {
                            _state.bri_inc = briInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error BRI incrementor must be between -254 to 254");
                        }
                    }
                }},
                {"sat=","Saturation of a light [0-254]", delegate(string v)
                {
                    byte sat;
                    if (byte.TryParse(v, out sat))
                        _state.sat = sat;
                    else
                    {
                        WriteMessageToConsole("Error SAT invalid value " + v);
                        _error = true;
                    }
                }},
                {"sat_inc=","Saturation incrementor [-254-254]", delegate(string v)
                {
                    if (short.TryParse(v, out var satInc))
                    {
                        if (satInc >= -254 && satInc <= 254)
                        {
                            _state.sat_inc = satInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error SAT incrementor must be between -254 to 254");
                        }
                    }
                }},
                {"ct=" ,"Colortemp of a light [153-500]", delegate(string v)
                {
                    ushort ct;
                    if (ushort.TryParse(v, out ct))
                        _state.ct = ct;
                    else
                    {
                        WriteMessageToConsole("Error CT invalid value " + v);
                        _error = true;
                    }
                }},
                {"ct_inc=","Color Temperature incrementor [-500-500]", delegate(string v)
                {
                    if (short.TryParse(v, out var ctInc))
                    {
                        if (ctInc >= -500 && ctInc <= 500)
                        {
                            _state.ct_inc = ctInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error CT incrementor must be between -500 to 500");
                        }
                    }
                }},
                {"hue=","Hue of a light [0-65534]", delegate(string v)
                {
                    ushort hue;
                    if (ushort.TryParse(v, out hue))
                    {
                        if(hue >= 0 && hue <= 65535)
                            _state.hue = hue;
                        else
                        {
                            WriteMessageToConsole("Error HUE invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error HUE invalid value " + v);
                        _error = true;
                    }
                }},
                {"hue_inc=","Hue incrementor [-65534-65534]", delegate(string v)
                {
                    if (int.TryParse(v, out var hueInc))
                    {
                        if (hueInc >= -65534 && hueInc <= 65534)
                        {
                            _state.hue_inc = hueInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error HUE incrementor must be between -65535 to 65535");
                        }
                    }
                }},
                {"on:" ,"On state of a light (true or false)", delegate(string v)
                {
                    if(v == null)
                    {
                        Light bresult = _bridge.GetObject<Light>(_id);
                        
                        if (bresult != null)
                        {
                            if(bresult.state.on != null && bresult.state.on.Value)
                            {
                                _state.on = false;
                            }
                            else
                            {
                                _state.on = true;
                            }
                        }
                        else
                        {
                            WriteMessageToConsole("Error getting light null");
                        }
                    }
                    else
                    {
                        bool on;
                        if (bool.TryParse(v, out on))
                            _state.on = on;
                        else
                        {
                            WriteMessageToConsole("Error ON invalid value " + v);
                            _error = true;
                        }
                    }
                }},
                {"fx|effect=","Effect of a light [none,colorloop]", delegate(string v)
                {
                    if (v == "colorloop" || v == "none")
                        _state.effect = v;
                    else
                    {
                        WriteMessageToConsole("Error EFFECT invalid value " + v);
                        _error = true;
                    }
                        
                }},
                {"alert=","Brightness of a light [none,select,lselect]", delegate(string v)
                {
                    if (v == "none" || v == "select" || v == "lselect")
                        _state.alert = v;
                    else
                    {
                        WriteMessageToConsole("ALERT invalid value " + v);
                        _error = true;
                    }
                }},
                {"x=","x coordinate of a light [0.000-1.000]", delegate(string v)
                {
                    decimal x;
                    if (decimal.TryParse(v, out x))
                    {
                        if (x >= 0 && x <= 1)
                        {
                            if (_state.xy == null)
                            {
                                _state.xy = new decimal[2];                              
                            }
                            _state.xy[0]= x;
                        }
                        else
                        {
                            WriteMessageToConsole("Error X invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error X invalid value " + v);
                        _error = true;
                    }
                }},
                {"x_inc=","X incrementor coordinate of a light [-0.500-0.500]", delegate(string v)
                {
                    if (decimal.TryParse(v, out var xInc))
                    {
                        if (xInc >= -0.500m && xInc <= 0.500m)
                        {
                            if (_state.xy_inc == null)
                            {
                                _state.xy_inc = new decimal[2]{0,0};
                            }
                            _state.xy[0]= xInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error X incrementor invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error X incrementor invalid value " + v);
                        _error = true;
                    }
                }},
                {"y=","Y coordinate of a light [0.000-1.000]", delegate(string v)
                {
                    decimal y;
                    if (decimal.TryParse(v, out y))
                    {
                        if (y >= 0 && y <= 1)
                        {
                            if (_state.xy == null)
                            {
                                _state.xy = new decimal[2];
                            }
                            _state.xy[1] = y;
                        }
                        else
                        {
                            WriteMessageToConsole("Error Y invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error Y invalid value " + v);
                        _error = true;
                    }
                }},
                {"y_inc=","Y incrementor coordinate of a light [-0.500-0.500]", delegate(string v)
                {
                    if (decimal.TryParse(v, out var yInc))
                    {
                        if (yInc >= -0.500m && yInc <= 0.500m)
                        {
                            if (_state.xy_inc == null)
                            {
                                _state.xy_inc = new decimal[2]{0,0};
                            }
                            _state.xy[0]= yInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error Y invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error Y invalid value " + v);
                        _error = true;
                    }
                }},
                {"tt|transitiontime=","Transition time of the light", delegate(ushort v)
                {
                    _state.transitiontime = v;
                }}
            };
            #endregion
            #region GROUP_OPTIONS
            GroupOpts = new OptionSet
            {
                {"bri=","Brightness of a group [0-254]", delegate(string v)
                {
                    byte bri;
                    if (byte.TryParse(v, out bri))
                        _action.bri = bri;
                    else
                    {
                        WriteMessageToConsole("BRI invalid value " + v);
                        _error = true;
                    }
                }},
                {"bri_inc=","Brightness incrementor [-254-254]", delegate(string v)
                {
                    if (short.TryParse(v, out var briInc))
                    {
                        if (briInc >= -254 && briInc <= 254)
                        {
                            _action.bri_inc = briInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error BRI incrementor must be between -254 to 254");
                        }
                    }
                }},
                {"sat=","Saturation of a group [0-254]", delegate(string v)
                {
                    byte sat;
                    if (byte.TryParse(v, out sat))
                        _action.sat = sat;
                    else
                    {
                        WriteMessageToConsole("SAT invalid value " + v);
                        _error = true;
                    }
                }},
                {"sat_inc=","Saturation incrementor [-254-254]", delegate(string v)
                {
                    if (short.TryParse(v, out var satInc))
                    {
                        if (satInc >= -254 && satInc <= 254)
                        {
                            _action.sat_inc = satInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error SAT incrementor must be between -254 to 254");
                        }
                    }
                }},
                {"ct=" ,"Colortemp of a group [153-500]", delegate(string v)
                {
                    ushort ct;
                    if (ushort.TryParse(v, out ct))
                        _action.ct = ct;
                    else
                        WriteMessageToConsole("CT invalid value " + v);
                }},
                {"ct_inc=","Color Temperature incrementor [-500-500]", delegate(string v)
                {
                    if (short.TryParse(v, out var ctInc))
                    {
                        if (ctInc >= -500 && ctInc <= 500)
                        {
                            _action.ct_inc = ctInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error CT incrementor must be between -500 to 500");
                        }
                    }
                }},
                {"hue=","Hue of a group [0-65534]", delegate(string v)
                {
                    ushort hue;
                    if (ushort.TryParse(v, out hue))
                    {
                        if(hue <= 65535)
                            _action.hue = hue;
                        else
                        {
                            WriteMessageToConsole("HUE invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("HUE invalid value " + v);
                        _error = true;
                    }
                }},
                {"hue_inc=","Hue incrementor [-65534-65534]", delegate(string v)
                {
                    if (int.TryParse(v, out var hueInc))
                    {
                        if (hueInc >= -65534 && hueInc <= 65534)
                        {
                            _action.hue_inc = hueInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error HUE incrementor must be between -65535 to 65535");
                        }
                    }
                }},
                {"on:" ,"On state of a group (true or false)", delegate(string v)
                {
                    if(v == null)
                    {
                        Group bresult = _bridge.GetObject<Group>(_id);
                        if (bresult != null)
                        {
                            if(bresult.action.on != null && bresult.action.on.Value)
                            {
                                _action.on = false;
                            }
                            else
                            {
                                _action.on = true;
                            }

                        }
                        else
                        {
                            WriteMessageToConsole("Error getting group : " + v);
                        }
                    }
                    else
                    {
                        bool on;
                        if (bool.TryParse(v, out on))
                            _action.on = on;
                        else
                        {
                            WriteMessageToConsole("ON invalid value " + v);
                            _error = true;
                        }
                    }
                }},
                {"fx|effect=","Effect of a group [none,colorloop]", delegate(string v)
                {
                    if (v == "colorloop" || v == "none")
                        _action.effect = v;
                    else
                    {
                        WriteMessageToConsole("EFFECT invalid value " + v);
                        _error = true;
                    }
                }},
                {"x=","x coordinate of a group [0.000-1.000]", delegate(string v)
                {
                    decimal x;
                    if (decimal.TryParse(v, out x))
                    {
                        if (x >= 0 && x <= 1)
                        {
                            if(_action.xy == null)
                                _action.xy = new decimal[2];

                            _action.xy[0] = x;
                        }
                        else
                        {
                            WriteMessageToConsole("X invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("X invalid value " + v);
                        _error = true;
                    }
                        
                }},
                {"x_inc=","X incrementor coordinate of a light [-0.500-0.500]", delegate(string v)
                {
                    if (decimal.TryParse(v, out var xInc))
                    {
                        if (xInc >= -0.500m && xInc <= 0.500m)
                        {
                            if (_action.xy_inc == null)
                            {
                                _action.xy_inc = new decimal[2]{0,0};
                            }
                            _action.xy[0]= xInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error X incrementor invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error X incrementor invalid value " + v);
                        _error = true;
                    }
                }},
                {"y=","Y coordinate of a group [0.000-1.000]", delegate(string v)
                {
                    decimal y;
                    if (decimal.TryParse(v, out y))
                    {
                        if (y >= 0 && y <= 1)
                        {
                            if(_action.xy == null)
                                _action.xy = new decimal[2];
                            _action.xy[1] = y;
                        }
                        else
                        {
                            WriteMessageToConsole("Y invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Y invalid value " + v);
                        _error = true;
                    }
                }},
                {"y_inc=","Y incrementor coordinate of a light [-0.500-0.500]", delegate(string v)
                {
                    if (decimal.TryParse(v, out var yInc))
                    {
                        if (yInc >= -0.500m && yInc <= 0.500m)
                        {
                            if (_action.xy_inc == null)
                            {
                                _action.xy_inc = new decimal[2]{0,0};
                            }
                            _action.xy[0]= yInc;
                        }
                        else
                        {
                            WriteMessageToConsole("Error Y invalid value " + v);
                            _error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error Y invalid value " + v);
                        _error = true;
                    }
                }},
                {"tt|transitiontime=","Transition time of the group", delegate(ushort v)
                {
                    _action.transitiontime = v;
                }}
            };
            #endregion
            #region CREATE_GROUP_OPTIONS
            CreateGrOpts = new OptionSet
            {
                {"n|name=","Name of the new group",delegate(string v)
                {
                    _grp.name = v;
                }},
                {"members=","ID of the lights in the group splitted by a comma. [eg:1,2,3]", delegate(string v)
                {                  
                    _grp.lights.AddRange(v.Split(',').ToArray());
                }}
            };
            #endregion
            #region SENSOR

            SensorOpts = new OptionSet
            {
                {"open=", "Open attribute of the sensor", delegate(string v)
                    {

                        ClipOpenCloseSensorState ss = new ClipOpenCloseSensorState();
                        bool open;
                        if (bool.TryParse(v, out open))
                        {
                            ss.open = open;
                            _sensorstate = ss;
                        }
                        else
                        {
                            WriteMessageToConsole($"open invalid value {v} expecting true or false.");
                        }

                    }

                },
                {"flag=","flag attribute of the sensor", delegate(string v)
                    {
                        ClipGenericFlagSensorState fs = new ClipGenericFlagSensorState();
                        bool flag;
                        if (bool.TryParse(v, out flag))
                        {
                            fs.flag = flag;
                            _sensorstate = fs;
                        }
                        else
                        {
                            WriteMessageToConsole($"flag invalid value {v} expecting true or false.");
                        }
                    }
                },
                {"status=","status attribute of the sensor", delegate(string v)
                    {
                        ClipGenericStatusSensorState gs = new ClipGenericStatusSensorState();
                        int status;
                        if (int.TryParse(v, out status))
                        {
                            gs.status = status;
                            _sensorstate = gs;
                        }
                        else
                        {
                            WriteMessageToConsole($"status invalid value {v} expecting integer");
                        }
                    }
                },
                {"humidity=","humidity attribute of the sensor", delegate(string v)
                    {
                        ClipHumiditySensorState hs = new ClipHumiditySensorState();
                        int humidity;
                        if (int.TryParse(v, out humidity))
                        {
                            hs.humidity = humidity;
                            _sensorstate = hs;
                        }
                        else
                        {
                            WriteMessageToConsole($"humidity invalid value {v} expecting integer.");
                        }
                    } 
                },
                {"presence=","Presence attribute of the sensor", delegate(string v)
                    {
                        PresenceSensorState ps = new PresenceSensorState();
                        bool presence;
                        if (bool.TryParse(v, out presence))
                        {
                            ps.presence = presence;
                            _sensorstate = ps;
                        }
                        else
                        {
                            WriteMessageToConsole($"presence invalid value {v} expecting true or false.");
                        }
                    }
                },
                {"temperature=","Temperature attribute of the sensor", delegate(string v)
                    {
                        TemperatureSensorState ts = new TemperatureSensorState();
                        int temperature;
                        if(int.TryParse(v,out temperature))
                        {
                            ts.temperature = temperature;
                            _sensorstate = ts;
                        }
                        else
                        {
                            WriteMessageToConsole($"temperature invalid value {v} expecting integer.");
                        }
                    }
                }
            };

            #endregion
        }

        private static bool PromptYesNo()
        {
            bool result = false;
            Console.Write(@"Are you sure [y/n] ? ");
            ConsoleKeyInfo key = Console.ReadKey();
            result = key.Key == ConsoleKey.Y;
            return result;          
        }

        private static void DetectLocalProxy(bool proxy)
        {
            Hue.DetectLocalProxy = proxy;
        }

        public static void ExecuteCommand(string[] args)
        {
            if (!args.Contains("-help"))
            {
                try
                {
                    List<string> extra = MainOpts.Parse(args);

                    if (!string.IsNullOrEmpty(WinHueSettings.bridges.DefaultBridge) && !string.IsNullOrWhiteSpace(WinHueSettings.bridges.DefaultBridge))
                    {
                        if(WinHueSettings.bridges.BridgeInfo.ContainsKey(WinHueSettings.bridges.DefaultBridge))
                        {
                            string mac = WinHueSettings.bridges.DefaultBridge;
                            var bridgeSettings = WinHueSettings.bridges.BridgeInfo[mac];
                            string ip = bridgeSettings.ip;
                            string name = bridgeSettings.name;
                            string apiKey = bridgeSettings.apikey;
                            _bridge = new Bridge(IPAddress.Parse(ip), mac, name, apiKey);
                            if (_bridge != null && _error == false)
                            {
                                extra = ObjOpts.Parse(extra);
                                switch (Cmd)
                                {
                                    case Command.Light:
                                        extra = LightOpts.Parse(extra);
                                        if (!_error)
                                            SetLightState();
                                        break;
                                    case Command.Group:
                                        extra = GroupOpts.Parse(extra);
                                        if (!_error)
                                            SetGroupAction();
                                        break;
                                    case Command.Scene:
                                        if (!_error)
                                        {
                                            _bridge.ActivateScene(_id);
                                            WriteMessageToConsole($"Setting scene {_id}...");                                    
                                        }
                                        break;
                                    case Command.Sensor:
                                        extra = SensorOpts.Parse(extra);
                                        if (!_error)
                                        {
                                            SetSensorState();
                                            
                                        }
                                        break;
                                    case Command.Creategroup:
                                        extra = CreateGrOpts.Parse(extra);
                                        if (!_error)
                                            CreateGroup();
                                        break;
                                    default:
                                        break;
                                }

                                if (extra.Count != 0)
                                {
                                    WriteMessageToConsole($"The following commands were ignored : {string.Join(",", extra)}");
                                }
                            }
                            else
                            {
                                ShowHelp();
                            }

                        }
                        else
                        {
                            WriteMessageToConsole("The default bridge has not been found. Please make sure the bridge is powered on and connected to the network.");
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Please pair at least one bridge with WinHue in order to use whc.");

                    }

                }
                catch(OptionException ex)
                {
                    Console.WriteLine($"Error : option {ex.OptionName} value missing or invalid. Please specify the value of this option or refer to the help by using -help .");
                }
                catch(ArgumentNullException)
                {
                    WriteMessageToConsole("The defaut bridge ip address is invalid.");
                }
                catch(FormatException)
                {
                    WriteMessageToConsole("The defaut bridge ip address is invalid.");
                }
            }
            else
            {
                ShowHelp();
            }
        }

        private static void ShowHelp()
        {
            Console.Clear();
            Console.WriteLine(@"Usage is : whc [Global Options] [Objet Options] [SubOptions]");
            Console.WriteLine("");
            Console.WriteLine(@"***** GLOBAL OPTIONS *******");
            MainOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"****** OBJECT OPTIONS *******");
            ObjOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"***** CREATE GROUP ******");
            CreateGrOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"****** SUB OPTIONS *******");
            Console.WriteLine(@"For LIGHTS :");
            LightOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine(@"For GROUPS :");
            GroupOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"****** SENSORS OPTIONS *******");
            SensorOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine(@"*** Rules, sensors, scenes, schedules cannot be created by console. Please use WinHue 3 desktop application to create them. ***");
            Console.WriteLine(@"*** You need to pair a bridge with WinHue gui before you send a command to the bridge for the first time.***");
        }

        private static void CreateGroup()
        {
            if (_grp.name != null && _grp.lights != null)
            {
                bool bresult = _bridge.CreateObject(_grp);
                if(bresult)
                {
                    WriteMessageToConsole("Groupe named " + _grp.name + " created succesfully");                    
                }
            }
            else
                WriteMessageToConsole("Error group creation requires a name and a list of lights.");
        }

        private static void SetGroupAction()
        {
            bool bresult = _bridge.SetState(_action, _id);
            
            if (!bresult || _error)
            {
                WriteMessageToConsole("An error occured while sending the group state to the bridge.");
            }
            else
                WriteMessageToConsole("Group action sent succesfully to group : " + _id);
        }
        
        private static void WriteMessageToConsole(string msg)
        {
            if(_nomsg == false)
            {
                Console.WriteLine(msg);
            }
        }

        private static bool CheckIdType(string objectid)
        {
            bool result = true;
            try
            {
                long number = Convert.ToInt64(objectid);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        private static void SetLightState()
        {
            bool bresult = _bridge.SetState(_state, _id);
            
            if (!bresult || _error)
            {
                Console.WriteLine(@"An error occured while sending the light state to the bridge.");
                Console.WriteLine(_bridge.LastCommandMessages);
            }
            else
                Console.WriteLine("Light state sent succesfully to light : " + _id);
        }

        private static void SetSensorState()
        {
            
            if (_sensorstate == null)
            {
                Console.WriteLine(@"Error : Please specify a sensor state.");
            }
            else
            {
                bool bresult = _bridge.ChangeSensorState(_id, _sensorstate);
                if (!bresult || _error)
                {
                    Console.WriteLine(@"An error occured while sending the sensor state to the bridge.");
                    Console.WriteLine(_bridge.LastCommandMessages);
                }
                else
                {
                    Console.WriteLine("Sensor state sent succesfully to sensor : " + _id);
                }

            }

        }

        /*
                public static void SetIpAddress(string ip)
                {
                    IPAddress ipaddress;

                    if (connect == true)
                    {
                        return;
                    }

                    if(IPAddress.TryParse(ip,out ipaddress))
                    {

                        if (ipaddress != null)
                        {
                            bridge = new Bridge() { IpAddress = ipaddress, ApiKey = Properties.Settings.Default.apikey };
                            bridge.OnMessageAdded += WriteMessagesToConsole;
                        }
                        else
                        {
                            Console.WriteLine(@"ERROR : You need to connect to the bridge from WinHue at least once before you can use the console.");
                            error = true;

                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error : IPAddress {ip} is invalid.");
                        error = true;
                    }
                }
          */

    }
}
