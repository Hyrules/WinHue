using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NDesk.Options;
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
using WinHue3.Settings;
using Action = WinHue3.Philips_Hue.HueObjects.GroupObject.Action;


namespace whc
{
    public static class ConsoleHandler
    {
  
        static Bridge bridge;
        internal enum Command { LIGHT, GROUP, SCENE, SCHEDULE, BRIDGE, NONE, CREATEGROUP, DELETEGROUP, SENSOR }
        internal static Command cmd;
        static OptionSet mainOpts;
        static OptionSet objOpts;
        static OptionSet lightOpts;
        static OptionSet groupOpts;
        static OptionSet createGrOpts;
        static OptionSet sensorOpts;
        static Group grp;
        static bool error;
        static State state;
        static Action action;
        static string id;
        static bool noprompt;
        static bool nomsg;
        static object sensorstate;

        static ConsoleHandler()
        {
      
            noprompt = false;
            nomsg = false;
            cmd = Command.NONE;
            #region MAIN_OPTIONS           
            mainOpts = new OptionSet
            {
                {"noprompt", "Set this option if you want to ignore prompts. (This will assume yes for all questions)", delegate(string v){noprompt = true;}},
                {"nomsg","Cancel the output of messages from the bridge.(The application will not output anything) ", delegate(string v){nomsg = true;}}
            };
            #endregion
            #region OBJECT_OPTIONS
            objOpts = new OptionSet
            {
                {"l|light=","Set the mode to light.", delegate(string v) 
                { 
                    if(cmd == Command.NONE)
                    {
                        cmd = Command.LIGHT;
                        error = !CheckIDType(v);
                        id = v;
                        state = new State();
                    }
                    else
                        error = true;
                }},
                {"g|group=","Set the mode to group.", delegate(string v) 
                { 
                    if(cmd == Command.NONE)
                    {
                        action = new Action();
                        error = !CheckIDType(v);
                        id = v;
                        cmd = Command.GROUP;
                    }
                    else
                        error = true;
                }},             
                {"sc|schedule=","Set the mode to schedule.", delegate(string v)
                { 
                    if(cmd == Command.NONE)
                    {
                        id = v;
                        cmd = Command.SCHEDULE;
                    }
                    else
                        error = true;
                }},
                {"sn|scene=","Set the mode to scene.", delegate(string v) 
                { 
                    if(cmd == Command.NONE)
                    {
                        id = v;
                        
                        cmd = Command.SCENE;
                    }
                    else
                        error = true;
                }},
                {"ss|sensor=","Set the mode to sensor.", delegate(string v)
                {
                    if(cmd == Command.NONE)
                    {
                        id = v;
                        error = !CheckIDType(v);
                        cmd = Command.SENSOR;
                    }
                    else
                        error = true;
                }},
                {"cg|creategroup","Create a new group.", delegate(string v)
                {
                    grp = new Group();
                    if (cmd == Command.NONE)
                        cmd = Command.CREATEGROUP;
                    else
                        error = true;
                }},
                {"dg|deletegroup=","Delete the selected group", delegate(string v)
                {
                    if (cmd == Command.NONE)
                    {
                        cmd = Command.DELETEGROUP;
                        if(PromptYesNo() || noprompt)
                        {
                            bool bresult = bridge.RemoveObject(new Group(){Id = v});
                            if (bresult)
                            {

                                WriteMessageToConsole($"Group {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting group :" + v);
                        }
                    }
                    else
                        error = true;                   
                }},
                {"dc|deleteschedule=","Delete the selected schedule", delegate(string v)
                {
                    if (cmd == Command.NONE)
                    {
                        if(PromptYesNo() || noprompt)
                        {
                            bool bresult = bridge.RemoveObject(new Schedule(){Id = v});
                            if (bresult)
                            {
                                WriteMessageToConsole($"Schedule {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting schedule :" + v);
                        }
                    }
                    else
                        error = true;                   
                }},
                {"dl|deletelight=","Delete the selected light", delegate(string v)
                {
                    if (cmd == Command.NONE)
                    {
                        if(PromptYesNo() || noprompt)
                        {
                            bool bresult = bridge.RemoveObject(new Light(){Id =v});         
                            if (bresult)
                            {
                                WriteMessageToConsole($"Light {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting light :" + v);
                        }
                    }
                    else
                        error = true;                   
                }},
                {"do|deletesensor=","Delete the selected sensor", delegate(string v)
                {
                    if (cmd == Command.NONE)
                    {
                        if(PromptYesNo() || noprompt)
                        {
                            bool bresult = bridge.RemoveObject(id,HueObjectType.sensors);
                            if (bresult)
                            {
                                WriteMessageToConsole($"Sensor {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting sensor :" + v);
                        }
                    }
                    else
                        error = true;                   
                }},
                {"dr|deleterule=","Delete the selected rule", delegate(string v)
                {
                    if (cmd == Command.NONE)
                    {
                        if(PromptYesNo() || noprompt)
                        {
                            bool bresult = bridge.RemoveObject(new Rule() {Id = v});
                            if (bresult)
                            {
                                WriteMessageToConsole($"Rule {v} deleted succesfully.");
                            }
                            else
                                WriteMessageToConsole("Error while deleting rule :" + v);
                        }
                    }
                    else
                        error = true;                   
                }},
                {"ll", "List the lights available in the bridge", delegate(string v)
                {
                    Dictionary<string,Light> bresult = bridge.GetListObjects<Light>();
                    if(bresult != null)
                    {
                        Dictionary<string, Light> listLights = bresult;
                        foreach(KeyValuePair<string,Light> kvp in listLights)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, ModelID={kvp.Value.modelid}, Type={kvp.Value.type}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of lights :" + bresult );
                    }
                }},
                {"lg", "List the groups available in the bridge", delegate(string v)
                {
                    Dictionary<string,Group> bresult = bridge.GetListObjects<Group>();
                    if(bresult != null)
                    {
                        Dictionary<string, Group> listgroups = bresult;
                        foreach(KeyValuePair<string,Group> kvp in listgroups)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, MembersID=[{string.Join(",", kvp.Value.lights)}], Type={kvp.Value.type}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of groups : " + bresult);
                    }
                }},
                {"ls", "List the scenes available in the bridge", delegate(string v)
                {
                    Dictionary<string,Scene> bresult = bridge.GetListObjects<Scene>();
                    if(bresult != null)
                    {
                        Dictionary<string, Scene> listscenes = bresult;
                        foreach(KeyValuePair<string,Scene> kvp in listscenes)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, MembersID=[{string.Join(",", kvp.Value.lights)}]");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of scenes : " + bresult);
                    }
                }},
                {"lc", "List the schedules available in the bridge", delegate(string v)
                {
                    Dictionary<string,Schedule> bresult = bridge.GetListObjects<Schedule>();
                    if(bresult != null)
                    {
                        foreach(KeyValuePair<string,Schedule> kvp in bresult)
                        {
                            WriteMessageToConsole($"[ID]={kvp.Key}, Name={kvp.Value.name}, Time={kvp.Value.localtime}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of schedules : " + bresult);
                    }
                }},
                {"lo", "List the sensors available in the bridge", delegate(string v)
                {
                    Dictionary<string,Sensor> bresult = bridge.GetListObjects<Sensor>();
                    
                    if(bresult != null)
                    {
                        
                        foreach(KeyValuePair<string,Sensor> kvp in bresult)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, Type={kvp.Value.type}, Model={kvp.Value.modelid}, SwVersion={kvp.Value.swversion}, Manufacturer={kvp.Value.manufacturername}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of sensors " + v);
                    }
                }},
                {"lr", "List the rules available in the bridge", delegate(string v)
                {
                    Dictionary<string,Rule> bresult = bridge.GetListObjects<Rule>();

                    if(bresult != null)
                    {
                        Dictionary<string, Rule> listrules = bresult;
                        foreach(KeyValuePair<string, Rule> kvp in listrules)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, Owner={kvp.Value.owner}, Status={kvp.Value.status}, TimesTriggered={kvp.Value.timestriggered}");
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
                        Light bresult = bridge.GetObject<Light>(v);
                        if (bresult != null)
                        {
                            Light light = bresult;
                            WriteMessageToConsole(light.state.ToString() ?? bridge.LastCommandMessages.ToString());

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
                        Group bresult = bridge.GetObject<Group>(v);
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
            lightOpts = new OptionSet
            {
                {"bri=","Brightness of a light [0-254]", delegate(string v)
                {
                    byte bri;
                    if (byte.TryParse(v, out bri))
                        state.bri = bri;
                    else
                    {
                        WriteMessageToConsole("Error BRI invalid value " + v);
                        error = true;
                    }
                }},
                {"sat=","Saturation of a light [0-254]", delegate(string v)
                {
                    byte sat;
                    if (byte.TryParse(v, out sat))
                        state.sat = sat;
                    else
                    {
                        WriteMessageToConsole("Error SAT invalid value " + v);
                        error = true;
                    }
                }},
                {"ct=" ,"Colortemp of a light [153-500]", delegate(string v)
                {
                    ushort ct;
                    if (ushort.TryParse(v, out ct))
                        state.ct = ct;
                    else
                    {
                        WriteMessageToConsole("Error CT invalid value " + v);
                        error = true;
                    }
                }},
                {"hue=","Hue of a light [0-65534]", delegate(string v)
                {
                    ushort hue;
                    if (ushort.TryParse(v, out hue))
                    {
                        if(hue >= 0 && hue <= 65535)
                            state.hue = hue;
                        else
                        {
                            WriteMessageToConsole("Error HUE invalid value " + v);
                            error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error HUE invalid value " + v);
                        error = true;
                    }
                }},
                {"on:" ,"On state of a light (true or false)", delegate(string v)
                {
                    if(v == null)
                    {
                        Light bresult = bridge.GetObject<Light>(id);
                        
                        if (bresult != null)
                        {
                            if(bresult.state.on != null && bresult.state.on.Value)
                            {
                                state.on = false;
                            }
                            else
                            {
                                state.on = true;
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
                            state.on = on;
                        else
                        {
                            WriteMessageToConsole("Error ON invalid value " + v);
                            error = true;
                        }
                    }
                }},
                {"fx|effect=","Effect of a light [none,colorloop]", delegate(string v)
                {
                    if (v == "colorloop" || v == "none")
                        state.effect = v;
                    else
                    {
                        WriteMessageToConsole("Error EFFECT invalid value " + v);
                        error = true;
                    }
                        
                }},
                {"alert=","Brightness of a light [none,select,lselect]", delegate(string v)
                {
                    if (v == "none" || v == "select" || v == "lselect")
                        state.alert = v;
                    else
                    {
                        WriteMessageToConsole("ALERT invalid value " + v);
                        error = true;
                    }
                }},
                {"x=","x coordinate of a light [0.000-1.000]", delegate(string v)
                {
                    decimal x;
                    if (decimal.TryParse(v, out x))
                    {
                        if (x >= 0 && x <= 1)
                        {
                            if (state.xy == null)
                            {
                                state.xy = new decimal[2];                              
                            }
                            state.xy[0]= x;
                        }
                        else
                        {
                            WriteMessageToConsole("Error X invalid value " + v);
                            error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error X invalid value " + v);
                        error = true;
                    }
                }},
                {"y=","Y coordinate of a light [0.000-1.000]", delegate(string v)
                {
                    decimal y;
                    if (decimal.TryParse(v, out y))
                    {
                        if (y >= 0 && y <= 1)
                        {
                            if (state.xy == null)
                            {
                                state.xy = new decimal[2];
                            }
                            state.xy[1] = y;
                        }
                        else
                        {
                            WriteMessageToConsole("Error Y invalid value " + v);
                            error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Error Y invalid value " + v);
                        error = true;
                    }
                }},
                {"tt|transitiontime=","Transition time of the light", delegate(ushort v)
                {
                    state.transitiontime = v;
                }}
            };
            #endregion
            #region GROUP_OPTIONS
            groupOpts = new OptionSet
            {
                {"bri=","Brightness of a group [0-254]", delegate(string v)
                {
                    byte bri;
                    if (byte.TryParse(v, out bri))
                        action.bri = bri;
                    else
                    {
                        WriteMessageToConsole("BRI invalid value " + v);
                        error = true;
                    }
                }},
                {"sat=","Saturation of a group [0-254]", delegate(string v)
                {
                    byte sat;
                    if (byte.TryParse(v, out sat))
                        action.sat = sat;
                    else
                    {
                        WriteMessageToConsole("SAT invalid value " + v);
                        error = true;
                    }
                }},
                {"ct=" ,"Colortemp of a group [153-500]", delegate(string v)
                {
                    ushort ct;
                    if (ushort.TryParse(v, out ct))
                        action.ct = ct;
                    else
                        WriteMessageToConsole("CT invalid value " + v);
                }},
                {"hue=","Hue of a group [0-65534]", delegate(string v)
                {
                    ushort hue;
                    if (ushort.TryParse(v, out hue))
                    {
                        if(hue <= 65535)
                            action.hue = hue;
                        else
                        {
                            WriteMessageToConsole("HUE invalid value " + v);
                            error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("HUE invalid value " + v);
                        error = true;
                    }
                }},
                {"on:" ,"On state of a group (true or false)", delegate(string v)
                {
                    if(v == null)
                    {
                        Group bresult = bridge.GetObject<Group>(id);
                        if (bresult != null)
                        {
                            if(bresult.action.on != null && bresult.action.on.Value)
                            {
                                action.on = false;
                            }
                            else
                            {
                                action.on = true;
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
                            action.on = on;
                        else
                        {
                            WriteMessageToConsole("ON invalid value " + v);
                            error = true;
                        }
                    }
                }},
                {"fx|effect=","Effect of a group [none,colorloop]", delegate(string v)
                {
                    if (v == "colorloop" || v == "none")
                        action.effect = v;
                    else
                    {
                        WriteMessageToConsole("EFFECT invalid value " + v);
                        error = true;
                    }
                }},
                {"x=","x coordinate of a group [0.000-1.000]", delegate(string v)
                {
                    decimal x;
                    if (decimal.TryParse(v, out x))
                    {
                        if (x >= 0 && x <= 1)
                        {
                            if(action.xy == null)
                                action.xy = new decimal[2];

                            action.xy[0] = x;
                        }
                        else
                        {
                            WriteMessageToConsole("X invalid value " + v);
                            error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("X invalid value " + v);
                        error = true;
                    }
                        
                }},
                {"y=","Y coordinate of a group [0.000-1.000]", delegate(string v)
                {
                    decimal y;
                    if (decimal.TryParse(v, out y))
                    {
                        if (y >= 0 && y <= 1)
                        {
                            if(action.xy == null)
                                action.xy = new decimal[2];
                            action.xy[1] = y;
                        }
                        else
                        {
                            WriteMessageToConsole("Y invalid value " + v);
                            error = true;
                        }
                    }
                    else
                    {
                        WriteMessageToConsole("Y invalid value " + v);
                        error = true;
                    }
                }},
                {"tt|transitiontime=","Transition time of the group", delegate(ushort v)
                {
                    action.transitiontime = v;
                }}
            };
            #endregion
            #region CREATE_GROUP_OPTIONS
            createGrOpts = new OptionSet
            {
                {"n|name=","Name of the new group",delegate(string v)
                {
                    grp.name = v;
                }},
                {"members=","ID of the lights in the group splitted by a comma. [eg:1,2,3]", delegate(string v)
                {                  
                    grp.lights.AddRange(v.Split(',').ToArray());
                }}
            };
            #endregion
            #region SENSOR

            sensorOpts = new OptionSet
            {
                {"open=", "Open attribute of the sensor", delegate(string v)
                    {

                        ClipOpenCloseSensorState ss = new ClipOpenCloseSensorState();
                        bool open;
                        if (bool.TryParse(v, out open))
                        {
                            ss.open = open;
                            sensorstate = ss;
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
                            sensorstate = fs;
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
                            sensorstate = gs;
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
                            sensorstate = hs;
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
                            sensorstate = ps;
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
                            sensorstate = ts;
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
                    List<string> extra = mainOpts.Parse(args);

                    if (!string.IsNullOrEmpty(WinHueSettings.settings.DefaultBridge) && !string.IsNullOrWhiteSpace(WinHueSettings.settings.DefaultBridge))
                    {
                        if(WinHueSettings.settings.BridgeInfo.ContainsKey(WinHueSettings.settings.DefaultBridge))
                        {
                            string ip = WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].ip;
                            bridge = new Bridge(IPAddress.Parse(ip), WinHueSettings.settings.DefaultBridge, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].apiversion, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].apiversion, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].swversion, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].apikey);
                            if (bridge != null && error == false)
                            {
                                extra = objOpts.Parse(extra);
                                switch (cmd)
                                {
                                    case Command.LIGHT:
                                        extra = lightOpts.Parse(extra);
                                        if (!error)
                                            SetLightState();
                                        break;
                                    case Command.GROUP:
                                        extra = groupOpts.Parse(extra);
                                        if (!error)
                                            SetGroupAction();
                                        break;
                                    case Command.SCENE:
                                        if (!error)
                                        {
                                            bridge.ActivateScene(id);
                                            WriteMessageToConsole($"Setting scene {id}...");                                    
                                        }
                                        break;
                                    case Command.SENSOR:
                                        extra = sensorOpts.Parse(extra);
                                        if (!error)
                                        {
                                            SetSensorState();
                                            
                                        }
                                        break;
                                    case Command.CREATEGROUP:
                                        extra = createGrOpts.Parse(extra);
                                        if (!error)
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
            mainOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"****** OBJECT OPTIONS *******");
            objOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"***** CREATE GROUP ******");
            createGrOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"****** SUB OPTIONS *******");
            Console.WriteLine(@"For LIGHTS :");
            lightOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine(@"For GROUPS :");
            groupOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("");
            Console.WriteLine(@"****** SENSORS OPTIONS *******");
            groupOpts.WriteOptionDescriptions(Console.Out);
            Console.WriteLine(@"*** Rules, sensors, scenes, schedules cannot be created by console. Please use WinHue 3 desktop application to create them. ***");
            Console.WriteLine(@"*** You need to pair a bridge with WinHue gui before you send a command to the bridge for the first time.***");
        }

        private static void CreateGroup()
        {
            if (grp.name != null && grp.lights != null)
            {
                bool bresult = bridge.CreateObject(grp);
                if(bresult)
                {
                    WriteMessageToConsole("Groupe named " + grp.name + " created succesfully");                    
                }
            }
            else
                WriteMessageToConsole("Error group creation requires a name and a list of lights.");
        }

        private static void SetGroupAction()
        {
            bool bresult = bridge.SetState(action, id);
            
            if (!bresult || error)
            {
                WriteMessageToConsole("An error occured while sending the group state to the bridge.");
            }
            else
                WriteMessageToConsole("Group action sent succesfully to group : " + id);
        }
        
        private static void WriteMessageToConsole(string msg)
        {
            if(nomsg == false)
            {
                Console.WriteLine(msg);
            }
        }

        private static bool CheckIDType(string objectid)
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
            bool bresult = bridge.SetState(state, id);
            
            if (!bresult || error)
            {
                Console.WriteLine(@"An error occured while sending the light state to the bridge.");
                Console.WriteLine(bridge.LastCommandMessages);
            }
            else
                Console.WriteLine("Light state sent succesfully to light : " + id);
        }

        private static void SetSensorState()
        {
            bool bresult = bridge.ChangeSensorState(id, sensorstate);
            
            if (!bresult || error)
            {
                Console.WriteLine(@"An error occured while sending the sensor state to the bridge.");
                Console.WriteLine(bridge.LastCommandMessages);
            }
            else
            {
                Console.WriteLine("Sensor state sent succesfully to sensor : " + id);
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
