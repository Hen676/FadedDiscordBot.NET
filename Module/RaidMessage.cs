using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FadedVanguardBot0._1.Events;
using FadedVanguardBot0._1.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FadedVanguardBot0._1.Module
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
            if (_config.Bot.Raid.Message.HasValue)
            {
                if (CachMessage.Id == _config.Bot.Raid.Message && !Reactions.reactionNames.Contains(Reaction.Emote.Name))
                {
                    if (CachMessage.HasValue)
                        await CachMessage.Value.RemoveReactionAsync(Reaction.Emote, Reaction.User.GetValueOrDefault());
                    else if (CachChannel.HasValue)
                    {
                        var message = await CachChannel.Value.GetMessageAsync(CachMessage.Id);
                        await message.RemoveReactionAsync(Reaction.Emote, Reaction.User.GetValueOrDefault());
                    }
                    else
                    {
                        var channel = await _discord.GetChannelAsync(CachChannel.Id);
                        if (channel is SocketTextChannel channel1)
                        {
                            var message = await channel1.GetMessageAsync(CachMessage.Id);
                            await message.RemoveReactionAsync(Reaction.Emote, Reaction.User.GetValueOrDefault());
                        }
                    }
                }
            }
        }

        [SlashCommand("force", "Force raid message command. Used to force a message update.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task MotdForceCommand()
        {
            await RespondAsync($"Raid message forced");
            await _raidMessageEvent.Invoke();
        }

        [SlashCommand("setup", "Raid weekly message setup command. Used to configure the if/where the message updates.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task RaidSetupCommand(SocketChannel channel = null, bool? update = null)
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

            // reply with the answer
            string startingstring = toggle ? "Updated" : "Current";
            await RespondAsync($"{startingstring} raid update command: [<#{_config.Bot.Raid.Channel}> and {_config.Bot.Raid.Update}]");
        }
    }
}
