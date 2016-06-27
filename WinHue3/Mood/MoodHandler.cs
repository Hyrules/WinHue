using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WinHue3
{
    public static class MoodHandler
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Constant mood file extention. (.md)
        /// </summary>
        const string MoodExt = "md";

        /// <summary>
        /// Delete an Mood file
        /// </summary>
        /// <param name="File">Effect file to delete</param>
        public static bool DeleteMood(string FileName)
        {
            bool result = false;
            try
            {
                File.Delete($@"moods\{FileName}.{MoodExt}");
                result = true;
            }
            catch (Exception e)
            {
                log.Info(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Load Mood file from the folder.
        /// </summary>
        public static List<Mood> LoadMoodsFiles()
        {
            List<Mood> moods = new List<Mood>();
            string[] fxList; // List of Effects
            try
            {
                fxList = Directory.GetFiles("moods", "*." + MoodExt);
            }
            catch (Exception)
            {
                DirectoryInfo df = Directory.CreateDirectory("moods"); // Directory Information for the created mood folder
                fxList = Directory.GetFiles(df.FullName, "*." + MoodExt);
                log.Error(GlobalStrings.Mood_Missing_Folder);
            }
            BinaryFormatter bf = new BinaryFormatter(); // Binary Formater to load effects

            foreach (string str in fxList)
            {
                try
                {
                    FileStream fs = new FileStream(str, FileMode.Open); // File stream for the mood File
                    Mood obj = (Mood)bf.Deserialize(fs);
                    moods.Add(obj);
                    fs.Close();
                }
                catch (Exception e)
                {
                    log.Error(GlobalStrings.Invalid_Mood_file + " : " + str + e.Message);
                }
            }
            return moods;
        }

        /// <summary>
        /// Save a mood to file.
        /// </summary>
        /// <param name="newMood"></param>
        /// <returns></returns>
        public static bool SaveMood(Mood newMood)
        {
            bool result = false;
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                FileStream fs = new FileStream($@"moods\{newMood.Name}.{MoodExt}", FileMode.Create);
                bf.Serialize(fs, newMood);
                fs.Close();
                result = true;
            }
            catch (Exception)
            {
                log.Error("Save Error");
            }
            return result;
        }

        /// <summary>
        /// Check if mood already exists.
        /// </summary>
        /// <param name="name">Name of the mood.</param>
        /// <returns>True or False</returns>
        public static bool MoodExists(string name)
        {
            return File.Exists($@"moods\{name}.{MoodExt}");
        }
    }

}
