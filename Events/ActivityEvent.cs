using Coravel.Invocable;
using Discord;
using Discord.WebSocket;
using FadedVanguardBot.Service;
using System;
using System.Threading.Tasks;

namespace FadedVanguardBot.Events
{
    public class ActivityEvent : IInvocable
    {
        private readonly DiscordSocketClient _discord;
        private readonly Gw2ApiHandler _gW2ApiHandler;
        private static int i = 0;

        public ActivityEvent(DiscordSocketClient discord, Gw2ApiHandler gW2ApiHandler)
        {
            _discord = discord;
            _gW2ApiHandler = gW2ApiHandler;
        }
        public Task Invoke()
        {
            i++;
            switch (i)
            {
                case 1:
                    _discord.SetActivityAsync(new Game($"on V{Program.version}", ActivityType.Playing, ActivityProperties.None));
                    return Task.CompletedTask;
                case 2:
                    _discord.SetActivityAsync(new Game("Henry, unable to spell 'Toast'.", ActivityType.Watching, ActivityProperties.None));
                    return Task.CompletedTask;
                case 3:
                    _discord.SetActivityAsync(new Game("with the new / comamnds!", ActivityType.Playing, ActivityProperties.None));
                    return Task.CompletedTask;
                case 4:
                    _discord.SetActivityAsync(new Game($"{_gW2ApiHandler.GetMemeberCount()} guildies!", ActivityType.Watching, ActivityProperties.None));
                    i = 0;
                    return Task.CompletedTask;
                default:
                    Console.WriteLine($"Actvity is OOB, {i}/4");
                    return Task.CompletedTask;
            }
        }
    }
}
