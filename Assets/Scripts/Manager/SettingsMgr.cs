using System.IO;
using Kirara;
using Newtonsoft.Json;
using UnityEngine;

namespace Manager
{
    public class Settings
    {
        public int MainVolume { get; set; }
        public int MusicVolume { get; set; }
        public int DialogVolume { get; set; }
        public int SFXVolume { get; set; }
    }

    public static class SettingsMgr
    {
        public static Settings settings { get; private set; }

        private static readonly Settings defaultSettings = new()
        {
            MainVolume = 10,
            MusicVolume = 10,
            DialogVolume = 10,
            SFXVolume = 10
        };

        private static string uid;

        private static void SetAudio()
        {
            AudioMgr.Instance.MainVolume = settings.MainVolume / 10f;
            AudioMgr.Instance.MusicVolume = settings.MusicVolume / 10f;
            AudioMgr.Instance.DialogVolume = settings.DialogVolume / 10f;
            AudioMgr.Instance.SFXVolume = settings.SFXVolume / 10f;
        }

        public static void Save()
        {
            SetAudio();

            string json = JsonConvert.SerializeObject(settings);

            string fileDir = Path.Combine(Application.persistentDataPath, "player", uid);
            string filePath = Path.Combine(fileDir, "player_settings.json");

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            File.WriteAllText(filePath, json);
        }

        public static void Init(string uid)
        {
            SettingsMgr.uid = uid;

            string filePath = Path.Combine(Application.persistentDataPath, "player", uid,
                "player_settings.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                settings = JsonConvert.DeserializeObject<Settings>(json);
            }
            else
            {
                settings = JsonConvert.DeserializeObject<Settings>(JsonConvert.SerializeObject(defaultSettings));
                Save();
            }
            SetAudio();
        }
    }
}