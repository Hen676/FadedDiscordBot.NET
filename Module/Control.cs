﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FadedVanguardBot._1.Models.Config;
using FadedVanguardBot0._1.Util;
using System;
using System.Threading.Tasks;

namespace FadedVanguardBot._1.Module
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

        private Task OnUserJoinedEvent(SocketGuildUser socketGuildUser)
        {
            if (_config.Bot.AutoRole.Toggle && _config.Bot.AutoRole.Role.HasValue)
                socketGuildUser.AddRoleAsync(_config.Bot.AutoRole.Role.Value);
            return Task.CompletedTask;
        }

        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        [SlashCommand("autorole", "Setup simple autorole command.")]
        public async Task AutoRole(SocketRole role = null, bool? on = null)
        {
            await Context.Channel.TriggerTypingAsync();
            bool toggle = false;

            if (role != null)
            {
                if (role.IsEveryone)
                {
                    await RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            $"Invalid Command", $"<@&{role.Id}> cannot be added"), ephemeral: true);
                    return;
                }
                if (role.Permissions.Administrator)
                {
                    await RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            $"Invalid Command", $"<@&{role.Id}> has administrator permision and can't be added"), ephemeral: true);
                    return;
                }
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
            await RespondAsync(
                embed: DiscordHelper.EmbedCreator(
                    $"{startingstring} autorole command", $"<@&{_config.Bot.AutoRole.Role}> and {_config.Bot.AutoRole.Toggle}"));
        }

        [SlashCommand("ping", "Simple ping command!")]
        public async Task Ping()
        {
            await Context.Channel.TriggerTypingAsync();
            string time = (DateTime.UtcNow - Context.Interaction.CreatedAt).Milliseconds.ToString();
            await RespondAsync(embed: DiscordHelper.EmbedCreator($"Pong <{time}ms>"));
        }
    }
}
