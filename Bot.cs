using Coravel;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using FadedBot.Service;
using FadedVanguardBot;
using FadedVanguardBot.Events;
using FadedVanguardBot.Models.Config;
using FadedVanguardBot.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FadedBot
{
    class Bot
    {
        private readonly Config _config;
        private DiscordSocketClient _discord;
        private InteractionService _commands;
        public static readonly Color color = new(0x968E6C);

        public Bot()
        {
            // Config
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: Program.IsDebug() ? "debug_config.json" : "config.json", optional: false)
                .Build()
                .Get<Config>();

            // Logger
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day);
            if (Program.IsDebug())
                log.MinimumLevel.Debug();
            else
                log.MinimumLevel.Information();
            Log.Logger = log.CreateLogger();
        }

        public async Task MainAsync()
        {
            // Create serivces
            var host = Host.CreateDefaultBuilder().ConfigureServices(ConfiguredService).Build();

            // Get serivces
            _discord = host.Services.GetRequiredService<DiscordSocketClient>();
            _commands = host.Services.GetRequiredService<InteractionService>();

            // Setup logging and the ready event
            _discord.Ready += ReadyAsync;
            _discord.Log += LogAsync;
            _commands.Log += LogAsync;

            // Launch client
            await _discord.LoginAsync(TokenType.Bot, _config.Bot.Token);
            await _discord.StartAsync();
            await host.Services.GetRequiredService<CommonHandler>().InitializeAsync();

            // Setup scheduler
            host.Services.UseScheduler(scheduler =>
            {
                var raidJob = scheduler.Schedule<WeeklyRaidTrainingEvent>();
                var motdJob = scheduler.Schedule<MotdEvent>();
                var activityJob = scheduler.Schedule<ActivityEvent>();

                if (Program.IsDebug())
                {
                    raidJob.EveryMinute();
                    motdJob.EveryMinute();
                    activityJob.EveryMinute();
                }
                else
                {
                    activityJob.EveryFiveMinutes();
                    raidJob.DailyAtHour(9).Tuesday();
                    motdJob.Hourly();
                }
            });

            host.Start();
            await Task.Delay(Timeout.Infinite);
        }

        private async Task ReadyAsync()
        {
            if (Program.IsDebug())
            {
                if (_config.Bot.ShardIdDebug.HasValue)
                {
                    Log.Debug($"In debug mode, adding commands to {_config.Bot.ShardIdDebug.Value}...");
                    await _commands.RegisterCommandsToGuildAsync(_config.Bot.ShardIdDebug.Value, true);
                }
                else
                {
                    Log.Error($"In debug mode, Unable to add commands due to invalid Shard Id");
                }
            }
            else
            {
                await _commands.RegisterCommandsGloballyAsync(true);
            }
            Log.Information($"Connected as -> [{_discord.CurrentUser}] :)");
        }

        private async Task LogAsync(LogMessage arg)
        {
            var severity = arg.Severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            Log.Write(severity, arg.Exception, "[{Source}] {Message}", arg.Source, arg.Message);
            await Task.CompletedTask;
        }
        private void ConfiguredService(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.AddScheduler()
                .AddTransient<WeeklyRaidTrainingEvent>()
                .AddTransient<MotdEvent>()
                .AddTransient<ActivityEvent>()
                .AddSingleton(_config)
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig 
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
                    MessageCacheSize = 10
                }))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<Gw2ApiHandler>()
                .AddSingleton<CommonHandler>()
                .BuildServiceProvider();
        }
    }
}