using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Classroom_Client
{
    public static class Settings
    {
        public class ProgramSettings
        {
            public string AuthPath { get; set; }
            public bool DefaultNotificatons { get; set; }
        }

        public static ProgramSettings settings;

        public static void RefreshSettings()
        {
            string SettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");
            if (!File.Exists(SettingsPath))
            {
                settings = new ProgramSettings()
                {
                    AuthPath = GetAuthPath(),
                    DefaultNotificatons = true
                };
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings));
            }
            else
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<ProgramSettings>(File.ReadAllText(SettingsPath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred reading settings file: {ex.ToString()}");
                    File.Delete(SettingsPath);
                    RefreshSettings();
                }
            }
        }

        private static string GetAuthPath()
        {
            var fileDialogue = new OpenFileDialog();
            fileDialogue.Title = "Open Auth file";
            fileDialogue.DefaultExt = "json";
            fileDialogue.Filter = "JSON files (*.json)|*.json";
            fileDialogue.FilterIndex = 1;
            fileDialogue.CheckFileExists = true;
            fileDialogue.CheckPathExists = true;
            fileDialogue.Multiselect = false;
            fileDialogue.ShowDialog();
            return String.IsNullOrEmpty(fileDialogue.FileName) ? GetAuthPath() : fileDialogue.FileName;
        }
    }
}