using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Channels;
using NDesk.Options;
using WinHue3;
using System.Text;
using Action = HueLib2.Action;
using Rule = HueLib2.Rule;
using HueLib2;

namespace whc
{
    public static class ConsoleHandler
    {
  
        static Bridge bridge;
        internal enum Command { LIGHT, GROUP, SCENE, SCHEDULE, BRIDGE, NONE, CREATEGROUP, DELETEGROUP, SENSOR };
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
        static SensorState sensorstate;

        static ConsoleHandler()
        {
      
            noprompt = false;
            nomsg = false;
            cmd = Command.NONE;
            #region MAIN_OPTIONS           
            mainOpts = new OptionSet()
            {
                {"noprompt", "Set this option if you want to ignore prompts. (This will assume yes for all questions)", delegate(string v){noprompt = true;}},
                {"nomsg","Cancel the output of messages from the bridge.(The application will not output anything) ", delegate(string v){nomsg = true;}},
            };
            #endregion
            #region OBJECT_OPTIONS
            objOpts = new OptionSet()
            {
                {"l|light=","Set the mode to light.", delegate(string v) 
                { 
                    if(cmd == Command.NONE)
                    {
                        cmd = Command.LIGHT;
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
                            CommandResult bresult = bridge.RemoveObject<Group>(v);

                            if (bresult.Success)
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
                            CommandResult bresult = bridge.RemoveObject<Schedule>(v);

                            if (bresult.Success)
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
                            CommandResult bresult = bridge.RemoveObject<Light>(v);

                            if (bresult.Success)
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
                        CommandResult bresult = bridge.RemoveObject<Sensor>(v);

                        if(PromptYesNo() || noprompt)
                        { 
                            if (bresult.Success)
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
                        CommandResult bresult = bridge.RemoveObject<Rule>(v);

                        if(PromptYesNo() || noprompt)
                        { 
                            if (bresult.Success)
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
                    
                    CommandResult bresult = bridge.GetListObjects<Light>();
                    if(bresult.Success)
                    {
                        Dictionary<string, Light> listLights = (Dictionary<string, Light>) bresult.resultobject;
                        foreach(KeyValuePair<string,Light> kvp in listLights)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, ModelID={kvp.Value.modelid}, Type={kvp.Value.type}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of lights.");
                    }
                }},
                {"lg", "List the groups available in the bridge", delegate(string v)
                {
                    CommandResult bresult = bridge.GetListObjects<Group>();

                    if(bresult.Success)
                    {
                        Dictionary<string, Group> listgroups = (Dictionary<string, Group>) bresult.resultobject;
                        foreach(KeyValuePair<string,Group> kvp in listgroups)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, MembersID=[{string.Join(",", kvp.Value.lights)}], Type={kvp.Value.type}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of groups.");
                    }
                }},
                {"ls", "List the scenes available in the bridge", delegate(string v)
                {
                    CommandResult bresult = bridge.GetListObjects<Light>();

                    if(bresult.Success)
                    {
                        Dictionary<string, Scene> listscenes = (Dictionary<string, Scene>)bresult.resultobject;
                        foreach(KeyValuePair<string,Scene> kvp in listscenes)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, MembersID=[{string.Join(",", kvp.Value.lights)}]");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of scenes.");
                    }
                }},
                {"lc", "List the schedules available in the bridge", delegate(string v)
                {
                    CommandResult bresult = bridge.GetListObjects<Schedule>();

                    if(bresult.Success)
                    {
                        Dictionary<string, Schedule> listscenes = (Dictionary<string, Schedule>)bresult.resultobject;

                        foreach(KeyValuePair<string,Schedule> kvp in listscenes)
                        {
                            WriteMessageToConsole($"[ID]={kvp.Key}, Name={kvp.Value.name}, Time={kvp.Value.localtime}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of schedules.");
                    }
                }},
                {"lo", "List the sensors available in the bridge", delegate(string v)
                {
                    CommandResult bresult = bridge.GetListObjects<Sensor>();
  
                    if(bresult.Success)
                    {
                        Dictionary<string, Sensor> listsensors = (Dictionary<string, Sensor>) bresult.resultobject;
                        foreach(KeyValuePair<string,Sensor> kvp in listsensors)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, Type={kvp.Value.type}, Model={kvp.Value.modelid}, SwVersion={kvp.Value.swversion}, Manufacturer={kvp.Value.manufacturername}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of sensors.");
                    }
                }},
                {"lr", "List the rules available in the bridge", delegate(string v)
                {
                    CommandResult bresult = bridge.GetListObjects<Rule>();

                    if(bresult.Success)
                    {
                        Dictionary<string, Rule> listrules = (Dictionary<string, Rule>)bresult.resultobject;
                        foreach(KeyValuePair<string, Rule> kvp in listrules)
                        {
                            WriteMessageToConsole(
                                $"[ID]={kvp.Key}, Name={kvp.Value.name}, Owner={kvp.Value.owner}, Status={kvp.Value.status}, TimesTriggered={kvp.Value.timestriggered}");
                        } 
                    }               
                    else
                    {
                        WriteMessageToConsole("An error occured while fetching the list of rules.");
                    }
                }},
                {"lightstate=","Get the state of a light", delegate(string v)
                {
                    byte value = 0;
                    bool isValidid = byte.TryParse(v,out value);

                    

                    if (isValidid && v != "0")
                    {
                        CommandResult bresult = bridge.GetObject<Light>(v);

                        if (bresult.Success)
                        {
                            Light light = (Light) bresult.resultobject;
                            WriteMessageToConsole(@light.state.ToString());
                        }
                        else
                        {
                            WriteMessagesToConsole();
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
                        CommandResult bresult = bridge.GetObject<Group>(v);

                        if (bresult.Success)
                        {
                            Group group = (Group) bresult.resultobject;
                            WriteMessageToConsole(@group?.action.ToString() ?? bridge.lastMessages.ToString());
                        }
                        else
                        {
                            WriteMessagesToConsole();
                        }
                    }
                    else
                    {
                        WriteMessageToConsole($"ERROR: The provided value {v} is not a valid id for the light. Please use a number between 1 and 255");
                    }

                }},

            };
            #endregion
            #region LIGHT_OPTIONS
            lightOpts = new OptionSet()
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
                        CommandResult bresult = bridge.GetObject<Light>(id);
                        if (bresult.Success)
                        {
                            Light light = (Light) bresult.resultobject;
                            if (light.state.@on != null && (bool) light.state.@on)
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
                            WriteMessagesToConsole();
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
                                state.xy = new XY();                              
                            }
                            state.xy = new XY {x = x};
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
                                state.xy = new XY();
                            }
                            state.xy.y = y;
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
                }},
            };
            #endregion
            #region GROUP_OPTIONS
            groupOpts = new OptionSet()
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
                        CommandResult bresult = bridge.GetObject<Group>(id);

                        if (bresult.Success)
                        {
                            Group group = (Group)bresult.resultobject;
                            if (@group.action.@on != null && (bool) @group.action.@on)
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
                            WriteMessagesToConsole();
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
                                action.xy = new XY();

                            action.xy.x = x;
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
                                action.xy = new XY();
                            action.xy.y = y;
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
                }},
            };
            #endregion
            #region CREATE_GROUP_OPTIONS
            createGrOpts = new OptionSet()
            {
                {"n|name=","Name of the new group",delegate(string v)
                {
                    grp.name = v;
                }},
                {"members=","ID of the lights in the group splitted by a comma. [eg:1,2,3]", delegate(string v)
                {                  
                    grp.lights.AddRange(v.Split(',').ToArray());
                }},
            };
            #endregion
            #region SENSOR

            sensorOpts = new OptionSet()
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
                        ClipGenericStatusState gs = new ClipGenericStatusState();
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
                        ClipPresenceSensorState ps = new ClipPresenceSensorState();
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
                        ClipTemperatureSensorState ts = new ClipTemperatureSensorState();
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
                            bridge = new Bridge(IPAddress.Parse(ip), WinHueSettings.settings.DefaultBridge, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].apiversion, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].apikey, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].swversion, WinHueSettings.settings.BridgeInfo[WinHueSettings.settings.DefaultBridge].apikey);
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
                CommandResult bresult = bridge.CreateObject<Group>(grp);
                if(bresult.Success)
                {
                    WriteMessageToConsole("Groupe named " + grp.name + " created succesfully");                    
                }
            }
            else
                WriteMessageToConsole("Error group creation requires a name and a list of lights.");
        }

        private static void SetGroupAction()
        {
            CommandResult bresult = bridge.SetState<Group>(action, id);

            if (!bresult.Success || error == true)
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

        private static void WriteMessagesToConsole()
        {
            if (nomsg != false) return;
            foreach (Message m in bridge.lastMessages)
            {
                Type typeofM = m.GetType();
                if (typeofM == typeof(Error))
                {
                    Error msg = (Error)m;
                    Console.WriteLine(@"ERROR : " + msg);
                }
                else if (typeofM == typeof(Success))
                {
                    Success msg = (Success)m;
                    Console.WriteLine(@"SUCCESS : " + msg);
                }
                else if (typeofM == typeof(CreationSuccess))
                {
                    CreationSuccess msg = (CreationSuccess)m;
                    Console.WriteLine(@"CREATION SUCCESS : " + msg.id);
                }
                else if (typeofM == typeof(DeletionSuccess))
                {
                    DeletionSuccess msg = (DeletionSuccess)m;
                    Console.WriteLine(@"DELETION SUCCESS : " + msg.success);
                }
            }
        }

        private static void SetLightState()
        {
            CommandResult bresult = bridge.SetState<Light>(state, id);
            
            if (!bresult.Success || error)
            {
                Console.WriteLine(@"An error occured while sending the light state to the bridge.");
                Console.WriteLine(bridge.lastMessages);
            }
            else
                Console.WriteLine("Light state sent succesfully to light : " + id);
        }

        private static void SetSensorState()
        {
            CommandResult bresult = bridge.ChangeSensorState(id,sensorstate);
            
            if (!bresult.Success || error)
            {
                Console.WriteLine(@"An error occured while sending the sensor state to the bridge.");
                Console.WriteLine(bridge.lastMessages);
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
