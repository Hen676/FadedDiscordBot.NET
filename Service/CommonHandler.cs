using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FadedVanguardBot.Util;
using Serilog;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace FadedBot.Service
{
    public class CommonHandler
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _commands;

        public CommonHandler(DiscordSocketClient discord, InteractionService commands, IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            _discord.InteractionCreated += HandleInteraction;

            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo, IInteractionContext interactionContext, IResult result)
        {
            if (!result.IsSuccess)
            {
                ErrorHandling(interactionContext, result);
            }
            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo contextCommandInfo, IInteractionContext interactionContext, IResult result)
        {
            if (!result.IsSuccess)
            {
                ErrorHandling(interactionContext, result);
            }
            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo slashCommandInfo, IInteractionContext interactionContext, IResult result)
        {
            if (!result.IsSuccess)
            {
                ErrorHandling(interactionContext, result);
            }
            return Task.CompletedTask;
        }

        private void ErrorHandling(IInteractionContext interactionContext, IResult result)
        {
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    interactionContext.Interaction.RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            "Error", "You are unable to access this command.\nCheck if you have the required guild permissions"), ephemeral: true
                            );
                    break;
                case InteractionCommandError.UnknownCommand:
                    interactionContext.Interaction.RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            "Error", "Command used is Unkown"), ephemeral: true
                            );
                    break;
                case InteractionCommandError.BadArgs:
                    interactionContext.Interaction.RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            "Error", "Command used has invalid arguments"), ephemeral: true
                            );
                    break;
                case InteractionCommandError.Exception:
                    interactionContext.Interaction.RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            "Error", "Command had an error during its operation"), ephemeral: true
                            );
                    break;
                case InteractionCommandError.Unsuccessful:
                    interactionContext.Interaction.RespondAsync(
                        embed: DiscordHelper.EmbedCreator(
                            "Error", "Command was unsuccessful"), ephemeral: true
                            );
                    break;
                default:
                    break;
            }
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(_discord, arg);
                await _commands.ExecuteCommandAsync(ctx, _provider);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                if (arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }
    }
}