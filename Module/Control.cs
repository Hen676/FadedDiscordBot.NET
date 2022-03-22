using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FadedVanguardBot0._1.Util;
using System;
using System.Threading.Tasks;

namespace FadedVanguardBot0._1.Module
{
    public class Control : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Config _config;
        private readonly DiscordSocketClient _discord;

        public Control(Config config, DiscordSocketClient discord)
        {
            _config = config;
            _discord = discord;
            _discord.UserJoined += OnUserJoinedEvent;
        }

        private Task OnUserJoinedEvent(SocketGuildUser arg)
        {
            if (_config.Bot.AutoRole.Toggle && _config.Bot.AutoRole.Role.HasValue)
                arg.AddRoleAsync(_config.Bot.AutoRole.Role.Value);
            return Task.CompletedTask;
        }

        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [SlashCommand("autorole", "Setup simple autorole command.")]
        public async Task AutoRole(SocketRole role = null, bool? on = null)
        {
            if (role.Permissions.Administrator)
            {
                await RespondAsync($"Autorole cannot use [{role.Name}] role as it has Administrator permission");
                return;
            }
            await Context.Channel.TriggerTypingAsync();
            bool toggle = false;

            if (role != null)
            {
                _config.Bot.AutoRole.Role = role.Id;
                toggle = true;
            }
            if (on.HasValue)
            {
                _config.Bot.AutoRole.Toggle = on.Value;
                toggle = true;
            }
            if (toggle)
                _config.SaveConfig();

            string startingstring = toggle ? "Updated" : "Current";
            await RespondAsync($"{startingstring} autorole update command: [@{_config.Bot.AutoRole.Role} and {_config.Bot.AutoRole.Toggle}]");
        }

        [SlashCommand("ping", "Simple ping command!")]
        public async Task Ping()
        {
            await Context.Channel.TriggerTypingAsync();
            string time = (DateTime.UtcNow - Context.Interaction.CreatedAt.UtcDateTime).TotalMilliseconds.ToString();
            await RespondAsync($"Pong: [{time}]");
        }
    }
}
