using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace FadedVanguardBot0._1.Util
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

    public class BotConfig
    {
        public AutoRoleConfig AutoRole { get; set; }
        public Gw2Config Gw2 { get; set; }
        public UpdateConfig Motd { get; set; }
        public UpdateConfig Raid { get; set; }
        public ulong? ShardIdDebug { get; set; }
        public string Token { get; set; }
        public string TokenDebug { get; set; }
    }

    public class AutoRoleConfig
    {
        public bool Toggle { get; set; }
        public ulong? Role { get; set; }
    }

    public class Gw2Config
    {
        public string ApiKey { get; set; }
        public string GuildId { get; set; }
    }

    public class UpdateConfig
    {
        public ulong? Channel { get; set; }
        public ulong? Message { get; set; }
        public bool Update { get; set; }
    }
}
