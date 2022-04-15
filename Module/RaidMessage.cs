using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FadedVanguardBot.Events;
using FadedVanguardBot.Models.Config;
using FadedVanguardBot.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FadedVanguardBot.Module
{
    [Group("raid", "Commands to edit Raid scheduled messages.")]
    public class RaidMessage : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Config _config;
        private readonly DiscordSocketClient _discord;
        private readonly RaidMessageEvent _raidMessageEvent;

        public RaidMessage(Config config, DiscordSocketClient discord, RaidMessageEvent raidMessageEvent)
        {
            _config = config;
            _discord = discord;
            _raidMessageEvent = raidMessageEvent;
            _discord.ReactionAdded += OnReactionAddedEvent;
        }

        private async Task OnReactionAddedEvent(Cacheable<IUserMessage, ulong> CachMessage, Cacheable<IMessageChannel, ulong> CachChannel, SocketReaction Reaction)
        {
            if (_discord.CurrentUser.Id == Reaction.UserId)
                return;
            if (_config.Bot.Raid.Message.HasValue)
            {
                if (CachMessage.Id == _config.Bot.Raid.Message && !Reactions.raidReactionNames.Contains(Reaction.Emote.Name))
                {
                    await CachMessage.GetOrDownloadAsync().Result.RemoveReactionAsync(Reaction.Emote, Reaction.User.GetValueOrDefault());
                }
            }
        }

        [SlashCommand("force", "Force raid message command. Used to force a message update.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task MotdForceCommand()
        {
            await RespondAsync(embed: DiscordHelper.EmbedCreator("Raid message forced"), ephemeral: true);
            await _raidMessageEvent.Invoke();
        }

        [SlashCommand("setup", "Raid weekly message setup command. Used to configure the if/where the message updates.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task RaidSetupCommand(SocketTextChannel channel = null, bool? update = null)
        {
            await Context.Channel.TriggerTypingAsync();
            bool toggle = false;

            if (channel != null)
            {
                _config.Bot.Raid.Channel = channel.Id;
                toggle = true;
            }
            if (update.HasValue)
            {
                _config.Bot.Raid.Update = update.Value;
                toggle = true;
            }
            if (toggle)
                _config.SaveConfig();

            string startingstring = toggle ? "Updated" : "Current";
            await RespondAsync(
                embed: DiscordHelper.EmbedCreator(
                    $"{startingstring} raid message", $"<#{_config.Bot.Raid.Channel}>\n{_config.Bot.Raid.Update}")
                );
        }
    }
}
