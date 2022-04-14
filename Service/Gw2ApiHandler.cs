using FadedVanguardBot.Models.Config;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace FadedVanguardBot.Service
{
    public class Gw2ApiHandler
    {
        private readonly Config _config;
        private readonly HttpClient _httpClient;

        public Gw2ApiHandler(Config config)
        {
            _config = config;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.guildwars2.com/v2/")
            };
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.Bot.Gw2.ApiKey}");
        }
        public string GetMemeberCount()
        {
            // Get guilds member_count from gw2 api
            var response = _httpClient.GetAsync($"guild/{_config.Bot.Gw2.GuildId}").Result;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return (string)JObject.Parse(response.Content.ReadAsStringAsync().Result)["member_count"];
        }

        public string GetMotd()
        {
            // Get guilds motd from gw2 api
            var response = _httpClient.GetAsync($"guild/{_config.Bot.Gw2.GuildId}").Result;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return (string)JObject.Parse(response.Content.ReadAsStringAsync().Result)["motd"];
        }

        public string GetRaidUrl()
        {
            // Get Raid icon url from gw2 api
            var response = _httpClient.GetAsync("files/map_raid_entrance").Result;
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return (string)JObject.Parse(response.Content.ReadAsStringAsync().Result)["icon"];
        }
    }
}
