using Coravel.Invocable;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using FadedVanguardBot.Models.Config;
using FadedVanguardBot.Service;
using FadedVanguardBot.Util;
using System.Threading.Tasks;

namespace FadedVanguardBot.Events
{
    public class RaidMessageEvent : IInvocable
    {
        private readonly Config _config;
        private readonly Gw2ApiHandler _gW2ApiHandler;
        private readonly DiscordSocketClient _discord;

        public RaidMessageEvent(Config config, Gw2ApiHandler gW2ApiHandler, DiscordSocketClient discord)
        {
            _config = config;
            _gW2ApiHandler = gW2ApiHandler;
            _discord = discord;
        }

        public Task Invoke()
        {
            if (_config.Bot.Raid.Update && _config.Bot.Raid.Channel.HasValue)
            {
                SocketTextChannel channel = (SocketTextChannel)_discord.GetChannel(_config.Bot.Raid.Channel.Value);
                if (_config.Bot.Raid.Message.HasValue)
                {
                    var oldmessage = channel.GetMessageAsync(_config.Bot.Raid.Message.Value).Result;
                    if (oldmessage != null)
                        oldmessage.DeleteAsync().Wait();
                }

                Embed embed = DiscordHelper.EmbedCreator(
                "Raid Training Vote",
                "Vote for which Raid Wing you want training for this week.\n" +
                "See Message of the Day to see when it's happening",
                _gW2ApiHandler.GetRaidUrl(), _discord.CurrentUser);
                RestUserMessage message = channel.SendMessageAsync(embed: embed).Result;
                foreach (string reaction in Reactions.raidReactionNames)
                {
                    message.AddReactionAsync(new Emoji(reaction)).Wait();
                }
                // TODO: Check reactions are added?
                _config.Bot.Raid.Message = message.Id;
                _config.SaveConfig();
            }
            return Task.CompletedTask;
        }
    }
}
