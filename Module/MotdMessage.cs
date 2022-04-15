using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FadedVanguardBot.Events;
using FadedVanguardBot.Models.Config;
using FadedVanguardBot.Util;
using System.Threading.Tasks;

namespace FadedVanguardBot.Module
{
    [Group("motd", "Commands to edit Message of the Day scheduled messages.")]
    public class MotdMessage : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Config _config;
        private readonly MotdMessageEvent _motdMessageEvent;

        public MotdMessage(Config config, MotdMessageEvent motdMessageEvent)
        {
            _config = config;
            _motdMessageEvent = motdMessageEvent;
        }

        [SlashCommand("force", "Force Message of the day command. Used to force a message update.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task MotdForceCommand()
        {
            await RespondAsync(embed: DiscordHelper.EmbedCreator("Message of the day forced"), ephemeral: true);
            await _motdMessageEvent.Invoke();
        }

        [SlashCommand("setup", "Message of the day setup command. Used to configure the if/where the message updates.")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
        [RequireOwner(Group = "Permission")]
        public async Task MotdSetupCommand(SocketTextChannel channel = null, bool? update = null)
        {
            await Context.Channel.TriggerTypingAsync();
            bool toggle = false;

            if (channel != null)
            {
                _config.Bot.Motd.Channel = channel.Id;
                toggle = true;
            }
            if (update.HasValue)
            {
                _config.Bot.Motd.Update = update.Value;
                toggle = true;
            }
            if (toggle)
                _config.SaveConfig();

            string startingstring = toggle ? "Updated" : "Current";
            await RespondAsync(
                embed: DiscordHelper.EmbedCreator(
                    $"{startingstring} Message of the Day message", $"<#{_config.Bot.Motd.Channel}>\n{_config.Bot.Motd.Update}")
                );
        }
    }
}
