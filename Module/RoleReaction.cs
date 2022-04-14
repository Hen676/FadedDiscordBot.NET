using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using FadedVanguardBot.Models.Config;
using FadedVanguardBot.Util;
using System;
using System.Threading.Tasks;

namespace FadedVanguardBot.Module
{
    [Group("reactrole", "Commands to add self asigning role message")]
    public class RoleReaction : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Config _config;
        private readonly DiscordSocketClient _discord;

        public RoleReaction(Config config, DiscordSocketClient discord)
        {
            _config = config;
            _discord = discord;
            _discord.ReactionAdded += OnReactionAddedEvent;
            _discord.ReactionRemoved += OnReactionRemovedEvent;
        }

        [SlashCommand("create", "Reation role create command. Used to create the the message. Use 'reactrole add' to setup themessage")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task RoleReactionCreateCommand(SocketTextChannel channel)
        {
            await Context.Channel.TriggerTypingAsync();

            if (_config.Bot.ReactionRole.Roles.Count <= 0)
            {
                await RespondAsync(
                    embed: DiscordHelper.EmbedCreator(
                        $"Invalid Command", $"Add roles using 'reactrole add' command"), ephemeral: true);
                return;
            }

            // Create message and send
            string desc = "";
            for (int i = 0; i < _config.Bot.ReactionRole.Roles.Count; i++)
            {
                desc += $"{Reactions.allReactionNames[i]} - <@&{_config.Bot.ReactionRole.Roles[i]}> \n\n";
            }
            RestUserMessage message = await channel.SendMessageAsync(embed: DiscordHelper.EmbedCreator("Self asign roles here!", desc, user: Context.Client.CurrentUser));
            _config.Bot.ReactionRole.Message = message.Id;
            _config.SaveConfig();
            // reply confirmation
            await RespondAsync(
                embed: DiscordHelper.EmbedCreator(
                    $"Created role reaction message", $"<#{channel.Id}>"), ephemeral: true);

            // Add reactions after since it's slow
            for (int i = 0; i < _config.Bot.ReactionRole.Roles.Count; i++)
            {
                message.AddReactionAsync(new Emoji(Reactions.allReactionNames[i])).Wait();
            }
        }

        [SlashCommand("add", "Add a role to reaction message. You are able to add up to 9 roles.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task RoleReactionAddCommand(SocketRole role)
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

            if (_config.Bot.ReactionRole.Roles.Contains(role.Id))
            {
                await RespondAsync(
                    embed: DiscordHelper.EmbedCreator(
                        $"Invalid Command", $"<@&{role.Id}> is already added"), ephemeral: true);
            }
            else
            {
                if (_config.Bot.ReactionRole.Roles.Count >= 9)
                {
                    await RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            $"Invalid Command", $"Max roles are added, please remove a role to add another"), ephemeral: true);
                }
                else
                {
                    // Add role and reply confirmation
                    _config.Bot.ReactionRole.Roles.Add(role.Id);
                    _config.SaveConfig();
                    await RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            $"Added role", $"<@&{role.Id}>"), ephemeral: true);
                }
            }
        }

        [SlashCommand("remove", "Remove a role to reaction message.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task RoleReactionRemoveCommand(SocketRole role)
        {
            if (_config.Bot.ReactionRole.Roles.Count < 1)
            {
                await RespondAsync(
                    embed: DiscordHelper.EmbedCreator(
                        $"Invalid Command", $"Zero roles are added, please add role before removing one"), ephemeral: true);
                return;
            }
            if (_config.Bot.ReactionRole.Roles.Contains(role.Id))
            {
                // Remove role and reply confirmation
                _config.Bot.ReactionRole.Roles.Remove(role.Id);
                _config.SaveConfig();
                await RespondAsync(
                    embed: DiscordHelper.EmbedCreator(
                        $"Removed role", $"<@&{role.Id}>"), ephemeral: true);
            }
            else
            {
                await RespondAsync(
                    embed: DiscordHelper.EmbedCreator(
                        $"Invalid Command", $"<@&{role.Id}> is not added"), ephemeral: true);
            }
        }


        private Task OnReactionRemovedEvent(Cacheable<IUserMessage, ulong> CachMessage, Cacheable<IMessageChannel, ulong> CachChannel, SocketReaction Reaction)
        {
            if (_discord.CurrentUser.Id == Reaction.UserId)
                return Task.CompletedTask;
            if (_config.Bot.ReactionRole.Message.HasValue)
            {
                if (CachMessage.Id == _config.Bot.ReactionRole.Message)
                {
                    int i = Array.IndexOf(Reactions.allReactionNames, Reaction.Emote.Name);
                    if (i >= 0 && i < _config.Bot.ReactionRole.Roles.Count)
                    {
                        IMessageChannel channel = CachChannel.GetOrDownloadAsync().Result;

                        SocketRole role = (channel as SocketGuildChannel).Guild.GetRole(_config.Bot.ReactionRole.Roles[i]);
                        (Reaction.User.Value as SocketGuildUser).RemoveRoleAsync(role);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private Task OnReactionAddedEvent(Cacheable<IUserMessage, ulong> CachMessage, Cacheable<IMessageChannel, ulong> CachChannel, SocketReaction Reaction)
        {
            if (_discord.CurrentUser.Id == Reaction.UserId)
                return Task.CompletedTask;
            if (_config.Bot.ReactionRole.Message.HasValue)
            {
                if (CachMessage.Id == _config.Bot.ReactionRole.Message)
                {
                    int i = Array.IndexOf(Reactions.allReactionNames, Reaction.Emote.Name);
                    if (i >= 0 && i < _config.Bot.ReactionRole.Roles.Count)
                    {
                        IMessageChannel channel = CachChannel.GetOrDownloadAsync().Result;

                        SocketRole role = (channel as SocketGuildChannel).Guild.GetRole(_config.Bot.ReactionRole.Roles[i]);
                        (Reaction.User.Value as SocketGuildUser).AddRoleAsync(role);
                    }
                    else
                    {
                        CachMessage.GetOrDownloadAsync().Result.RemoveReactionAsync(Reaction.Emote, Reaction.User.GetValueOrDefault());
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
