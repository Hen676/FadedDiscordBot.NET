using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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

        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                ErrorHandling(arg3);
            }
            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                ErrorHandling(arg3);
            }
            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg, IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {

                ErrorHandling(arg3);
            }
            return Task.CompletedTask;
        }

        private void ErrorHandling(IResult arg)
        {
            switch (arg.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
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