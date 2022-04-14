using Newtonsoft.Json;
using System;
using System.IO;

namespace FadedVanguardBot._1.Models.Config
{
    public class Config
    {
        public BotConfig Bot { get; set; }

        public void SaveConfig()
        {
            string json = JsonConvert.SerializeObject(this);
            var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "config.json");
            File.WriteAllText(appSettingsPath, json);
        }
    }
}
