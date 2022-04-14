using Discord;
using Discord.WebSocket;
using FadedBot;

namespace FadedVanguardBot0._1.Util
{
    public class DiscordHelper
    {
        public static Embed EmbedCreator(string title, string desc = null, string thumburl = null, SocketSelfUser user = null)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle(title);
            if (desc != null)
                embed.WithDescription(desc);
            if (thumburl != null)
                embed.WithThumbnailUrl(thumburl);
            if (user != null)
                embed.WithAuthor(user);
            embed.WithColor(Bot.color);
            embed.WithCurrentTimestamp();
            return embed.Build();
        }
    }
}
