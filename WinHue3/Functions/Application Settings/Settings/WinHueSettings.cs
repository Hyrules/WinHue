using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using WinHue3.Functions.RoomMap;

namespace WinHue3.Functions.Application_Settings.Settings
{
    public static class WinHueSettings
    {
        public static CustomSettings settings = new CustomSettings();
        public static CustomHotkeys hotkeys = new CustomHotkeys();
        public static CustomBridges bridges = new CustomBridges();

        private static readonly string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string floorplanpath = Path.Combine(path, "WinHue\\Floorplans");

        static WinHueSettings()
        {
            
            LoadAllSettings();

        }

        public static bool SaveFloorPlan(Floor floorplan)
        {
            
            try
            {
                if (!Directory.Exists(floorplanpath))
                {
                    log.Info("Floorplan folder does not exists trying to create it...");
                    DirectoryInfo di = Directory.CreateDirectory(floorplanpath);
                    log.Info("Floorplan folder created.");
                }

                log.Info($"Saving floorplan {floorplan.Name} to file...");
                string json = JsonConvert.SerializeObject(floorplan, Formatting.Indented);
                File.WriteAllText(Path.Combine(floorplanpath, floorplan.Name + ".flp"), json);
                log.Info("Saving completed.");

            }
            catch (Exception e)
            {
                log.Error("An error occured while saving the floorplan");
                log.Error(e.Message);
                return false;
            }

            return true;
        }

        public static List<Floor> LoadFloorPlans()
        {
            List<Floor> floors = new List<Floor>();
            try
            {
                log.Info("Trying to load floorplans...");
                if (!Directory.Exists(floorplanpath))
                {
                    log.Warn("No floor folder exists no floorplan to load.");

                }
                else
                {
                    IEnumerable<string> floorplanlist = Directory.EnumerateFiles(floorplanpath, "*.flp");
                    foreach (string file in floorplanlist)
                    {
                        log.Info($"Loading floorplan {file}...");
                        string json = File.ReadAllText(file);
                        Floor newfloor = JsonConvert.DeserializeObject<Floor>(json);
                        newfloor.AcceptChanges();
                        floors.Add(newfloor);

                    }
                }

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return new List<Floor>();
            }
            return floors;
        }

        public static bool FloorPlanExists(string name)
        {
            string filepath = Path.Combine(floorplanpath, name + ".flp");
            return File.Exists(filepath);
        }

        public static bool DeleteFloorPlan(Floor floor)
        {
            return DeleteFloor(floor.Name);
        }

        private static bool DeleteFloor(string name)
        {
            try
            {
                string filepath = Path.Combine(floorplanpath, name + ".flp");
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    log.Info($"Deleting {filepath}.");
                }
                else
                {
                    log.Warn($"File {filepath} does not exists. Not deleting.");
                    return false;
                }

            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }

            return true;
        }

        public static bool DeleteFloorPlan(string name)
        {
            return DeleteFloor(name);
        }

        public static bool ReplaceBridgeIp(string mac, IPAddress ip)
        {
            try
            {
                bridges.BridgeInfo[mac].ip = ip.ToString();
                SaveBridges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Load all settings.
        /// </summary>
        /// <returns>True or false on all success</returns>
        public static bool LoadAllSettings()
        {
            bool loadbridges = LoadBridges();
            bool loadhotkeys = LoadHotkeys();
            bool loadsettings = LoadSettings();
            if (loadbridges && loadhotkeys && loadsettings) return true;
            return false;
        }

        /// <summary>
        /// Save all settings
        /// </summary>
        /// <returns>True or false on all success</returns>
        public static bool SaveAllSettings()
        {
            bool savebridges = SaveBridges();
            bool savehotkeys = SaveHotkeys();
            bool savesettings = SaveSettings();
            if (savebridges && savehotkeys && savesettings) return true;
            return false;
        }

        /// <summary>
        /// Create the WinHue folders in the Application data folder.
        /// </summary>
        /// <returns>True or false on success</returns>
        private static bool CreateWinHueDirectory()
        {
            try
            {
                if (!Directory.Exists(Path.Combine(path, "WinHue")))
                {
                    log.Debug($@"Trying to create WinHue directory in {Path.Combine(path, "WinHue")}");
                    Directory.CreateDirectory(Path.Combine(path, "WinHue"));
                    log.Debug("Directory created succesfully.");
                    return true;
                }
                else
                {
                    log.Info("WinHue Folder already exists.");
                }
                return true;
            }
            catch (Exception)
            {
                log.Error($"Unable to created WinHue directory in {path}. Please check that you have the permissions to this path.");
                return false;
            }

        }

        /// <summary>
        /// Save the Hotkeys to file.
        /// </summary>
        /// <returns>True or false on success</returns>
        public static bool SaveHotkeys()
        {
            bool ret = false;
            try
            {
                string result = JsonConvert.SerializeObject(hotkeys, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented});
                log.Debug($@"Saving hotkeys : {hotkeys}");
                string filepath = Path.Combine(path, "WinHue\\WinHueHotkeys.set");
                if(File.Exists(filepath))
                {
                    log.Info("Backuping WinHueHotkeys.set file to WinHueHotkeys.set.bak");
                    File.Copy(filepath, filepath + ".bak", true);
                }

                if (CreateWinHueDirectory())
                {
                    File.WriteAllText(filepath, result);
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                hotkeys = new CustomHotkeys();
                ret = false;
                log.Error("Error while saving the hotkeys.", ex);

            }
            return ret;
        }

        /// <summary>
        /// Load hotkeys from file
        /// </summary>
        /// <returns>True or false on success</returns>
        public static bool LoadHotkeys()
        {
            bool result = false;

            try
            {
                string filepath = Path.Combine(path, "WinHue\\WinHueHotkeys.set");
                if (!File.Exists(filepath)) return result;
                log.Debug("Trying to open hotkeys file...");
                StreamReader sr = File.OpenText(filepath);
                log.Debug("File open.");
                string settingsString = sr.ReadToEnd();
                if (settingsString == string.Empty)
                {
                    log.Warn("Hotkey settings file is empty. It will be ignored.");
                    return false;
                }
                log.Debug($@"Loading hotkeys : {settingsString}");
                sr.Close();
                log.Debug("Deserializing the settings file.");
                hotkeys = JsonConvert.DeserializeObject<CustomHotkeys>(settingsString, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                result = true;
            }
            catch (Exception ex)
            {
                hotkeys = new CustomHotkeys();
                result = false;
                log.Error("Error while loading the hotkeys.", ex);
            }
            return result;
        }

        /// <summary>
        /// Save Bridge to file.
        /// </summary>
        /// <returns>True or false on success</returns>
        public static bool SaveBridges()
        {
            bool ret = false;
            try
            {
                string result = JsonConvert.SerializeObject(bridges, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
                log.Debug($@"Saving bridge settings : {bridges}");
                string filepath = Path.Combine(path, "WinHue\\WinHueBridges.set");
                if (File.Exists(filepath))
                {
                    log.Info("Backuping WinHueBridge.set file to WinHueBridge.set.bak");
                    File.Copy(filepath, filepath + ".bak", true);
                }
                
                if (CreateWinHueDirectory())
                {
                    File.WriteAllText(filepath, result);
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                bridges = new CustomBridges();
                ret = false;
                log.Error("Error while saving the bridge settings.", ex);

            }
            return ret;
        }

        /// <summary>
        /// Load bridge from file.
        /// </summary>
        /// <returns></returns>
        public static bool LoadBridges()
        {
            bool result = false;

            try
            {
                string filepath = Path.Combine(path, "WinHue\\WinHueBridges.set");
                if (!File.Exists(filepath)) return result;
               // if (new FileInfo(filepath).Length == 0) return result;
                log.Debug("Trying to open bridge settings file...");
                StreamReader sr = File.OpenText(filepath);
                log.Debug("File open.");
                string settingsString = sr.ReadToEnd();
                if (settingsString == string.Empty)
                {
                    log.Warn("Bridge settings file is empty. It will be ignored.");
                    return false;
                } 
                log.Debug($@"Loading bridge settings : {settingsString}");
                sr.Close();
                log.Debug("Deserializing the settings file.");
                bridges = JsonConvert.DeserializeObject<CustomBridges>(settingsString, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                result = true;
            }
            catch (Exception ex)
            {
                bridges = new CustomBridges();
                result = false;
                log.Error("Error while loading the bridge settings.", ex);
            }
            return result;
        }

        /// <summary>
        /// Save the settings to disk.
        /// </summary>
        /// <returns>True or false succeeded.</returns>
        public static bool SaveSettings()
        {
            bool ret = false;
            try
            {
                string result = JsonConvert.SerializeObject(settings ,new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented });
                log.Debug($@"Saving settings : {settings}");
                string filepath = Path.Combine(path, "WinHue\\WinHueSettings.set");
                if(File.Exists(filepath))
                {
                    log.Info("Backuping WinHueSettings.set file to WinHueSettings.set.bak");
                    File.Copy(filepath, filepath + ".bak", true);
                }
                
                if (CreateWinHueDirectory())
                {
                    File.WriteAllText(filepath, result);
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                settings = new CustomSettings();
                ret = false;
                log.Error("Error while saving the settings.",ex);

            }
            return ret;
        }

        /// <summary>
        /// Reload the settings from disk.
        /// </summary>
        /// <returns>True or false succeeded.</returns>
        public static bool LoadSettings()
        {
            bool result = false;
            
            try
            {
                string filepath = Path.Combine(path, "WinHue\\WinHueSettings.set");
                if (!File.Exists(filepath)) return result;
                log.Debug("Trying to open settings file...");
                StreamReader sr = File.OpenText(filepath);
                log.Debug("File open.");
                string settingsString = sr.ReadToEnd();
                if (settingsString == string.Empty)
                {
                    log.Warn("WinHue settings file is empty. It will be ignored.");
                    return false;
                }
                log.Debug($@"Loading settings : {settingsString}");
                sr.Close();         
                log.Debug("Deserializing the settings file.");    
                settings =  JsonConvert.DeserializeObject<CustomSettings>(settingsString,new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
                result = true;
            }
            catch (Exception ex)
            {
                settings = new CustomSettings();
                result = false;
                log.Error("Error while loading the settings.",ex);
            }
            return result;
        }
    }



}