using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace RssFeedMonitor
{
    public static class RssFeedAlertHandler
    {
        private static string ext = "ra";
        private static string folder = "rssalert";

        static RssFeedAlertHandler()
        {
            CheckFolderExists();
        }

        public static List<Alert> LoadRssFeedAlerts()
        {
            List<Alert> listConditions = new List<Alert>();
            XmlSerializer xser = new XmlSerializer(typeof(Alert));
            string[] listWa;

            try
            {
                listWa = Directory.GetFiles(folder, "*." + ext);
            }
            catch (Exception)
            {
                DirectoryInfo df = Directory.CreateDirectory(folder); // Directory Information for the created WeatherAlert folder
                listWa = Directory.GetFiles(df.FullName, "*." + ext);
            }

            foreach (string s in listWa)
            {
                try
                {
                    FileStream fs = new FileStream(s, FileMode.Open);
                    Alert cond = (Alert)xser.Deserialize(fs);
                    listConditions.Add(cond);
                    fs.Close();
                }
                catch (Exception)
                {
                    
                    
                }
            }

            return listConditions;
                
        }

        public static bool SaveFeedAlert(Alert condition)
        {
            bool result = false;
            try
            {
                CheckFolderExists();
                FileStream fs = File.Create(folder + "\\" + condition.Name + "." + ext);
                XmlSerializer xser = new XmlSerializer(condition.GetType());
                xser.Serialize(fs, condition);
                fs.Close();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static bool DeleteFeedAlert(Alert alert)
        {
            bool result = false;
            try
            {
                File.Delete(folder +"/"+ alert.Name + "." + ext);
                result = true;
            }
            catch (Exception)
            {
                result = false;

            }
            return result;

        }

        public static bool DeleteFeedAlert(string alertname)
        {
            bool result = false;
            try
            {

                File.Delete(folder + "/" + alertname + "." + ext);
                result = true;
            }
            catch (Exception)
            {
                result = false;

            }
            return result;

        }
        public static bool RssFeedAlertExists(string name)
        {
            return File.Exists(folder + "\\" + name + "." + ext);
        }

        private static bool CheckFolderExists()
        {
            bool result = true;
            if (Directory.Exists(folder)) return result;

            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception)
            {
                result = false;
            }
            
            return result;
        }
    }
}