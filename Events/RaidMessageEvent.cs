using Coravel.Invocable;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using FadedVanguardBot0._1.Service;
using FadedVanguardBot0._1.Util;
using System.Threading.Tasks;

namespace FadedVanguardBot0._1.Events
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
                RestUserMessage message = channel.SendMessageAsync(embed: _gW2ApiHandler.GetRaidEmbed()).Result;
                foreach (string reaction in Reactions.reactionNames)
                {
                    message.AddReactionAsync(new Emoji(reaction)).Wait();
                }
                _config.Bot.Raid.Message = message.Id;
                _config.SaveConfig();
            }
            return Task.CompletedTask;
        }
    }
}
