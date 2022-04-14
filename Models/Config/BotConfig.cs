using FadedVanguardBot0._1.Models.Config;

namespace FadedVanguardBot._1.Models.Config
{
    public class BotConfig
    {
        public AutoRoleConfig AutoRole { get; set; }
        public Gw2Config Gw2 { get; set; }
        public UpdateConfig Motd { get; set; }
        public UpdateConfig Raid { get; set; }
        public ReactionRoleConfig ReactionRole { get; set; }
        public ulong? ShardIdDebug { get; set; }
        public string Token { get; set; }
    }
}
