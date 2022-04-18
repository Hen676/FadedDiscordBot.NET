using Coravel.Invocable;
using Discord.WebSocket;
using FadedVanguardBot.Models.Config;
using FadedVanguardBot.Service;
using System;
using System.Threading.Tasks;

namespace FadedVanguardBot.Events
{
    public class MotdEvent : IInvocable
    {
        private readonly Config _config;
        private readonly Gw2ApiHandler _gW2ApiHandler;
        private readonly DiscordSocketClient _discord;

        public MotdEvent(Config config, Gw2ApiHandler gW2ApiHandler, DiscordSocketClient discord)
        {
            _config = config;
            _gW2ApiHandler = gW2ApiHandler;
            _discord = discord;
        }

        public Task Invoke()
        {
            if (_config.Bot.Motd.Update && _config.Bot.Motd.Channel.HasValue)
            {
                SocketTextChannel channel = (SocketTextChannel)_discord.GetChannel(_config.Bot.Motd.Channel.Value);
                string motd = _gW2ApiHandler.GetMotd();
                if (_config.Bot.Motd.Message.HasValue)
                {
                    var message = channel.GetMessageAsync(_config.Bot.Motd.Message.Value).Result;
                    if (message == null)
                    {
                        _config.Bot.Motd.Message = channel.SendMessageAsync(motd).Result.Id;
                        _config.SaveConfig();
                        return Task.CompletedTask;
                    }
                    else if (string.Equals(message.Content, motd, StringComparison.Ordinal))
                    {
                        channel.ModifyMessageAsync(message.Id, modify =>
                        {
                            modify.Content = motd;
                        }).Wait();
                    }
                }
                else
                {
                    _config.Bot.Motd.Message = channel.SendMessageAsync(motd).Result.Id;
                    _config.SaveConfig();
                }
            }
            return Task.CompletedTask;
        }
    }
}
