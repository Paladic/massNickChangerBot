using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NicknameChangerBot.Commands;
using NicknameChangerBot.Services;
using Serilog;
using Serilog.Events;

namespace NicknameChangerBot
{
    class Program
    {
#pragma warning disable CS8618
        public static DiscordSocketClient client;
#pragma warning restore CS8618
        static void Main ( string[] args )
        {
            // One of the more flexable ways to access the configuration data is to use the Microsoft's Configuration model,
            // this way we can avoid hard coding the environment secrets. I opted to use the Json and environment variable providers here.
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            DiscordSocketConfig cf = new DiscordSocketConfig
            {
                 MessageCacheSize = 100, 
                 AlwaysDownloadUsers = true, 
                 LogLevel = LogSeverity.Info,
                 GatewayIntents = GatewayIntents.Guilds |
                                  GatewayIntents.GuildMembers |
                                  GatewayIntents.GuildBans |
                                  GatewayIntents.GuildEmojis |
                                  GatewayIntents.GuildIntegrations |
                                  GatewayIntents.GuildWebhooks |
                                  GatewayIntents.GuildVoiceStates |
                                  GatewayIntents.GuildMessages |
                                  GatewayIntents.GuildMessageReactions |
                                  GatewayIntents.GuildMessageTyping |
                                  GatewayIntents.DirectMessages |
                                  GatewayIntents.DirectMessageReactions |
                                  GatewayIntents.DirectMessageTyping,

            };

            RunAsync(cf, config).GetAwaiter().GetResult();
        }

        static async Task RunAsync (DiscordSocketConfig cf, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            // Dependency injection is a key part of the Interactions framework but it needs to be disposed at the end of the app's lifetime.
            using var services = ConfigureServices(cf);

            client = services.GetRequiredService<DiscordSocketClient>();
            var commands = services.GetRequiredService<InteractionService>();
            client.Log += LogAsync;
            commands.Log += LogAsync;

            // Slash Commands and Context Commands are can be automatically registered, but this process needs to happen after the client enters the READY state.
            // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands. To determine the method we should
            // register the commands with, we can check whether we are in a DEBUG environment and if we are, we can register the commands to a predetermined test guild.
            client.Ready += async ( ) =>
            {
                //await client.Rest.DeleteAllGlobalCommandsAsync();
                if (IsDebug())
                {
                    await commands.RegisterCommandsToGuildAsync(948234282961354763, true);
                }
                else
                {
                    await commands.RegisterCommandsGloballyAsync(true);
                }
            };

            // Here we can initialize the service that will register and execute our commands
            await services.GetRequiredService<ClientHandler>().InitializeAsync();
            await services.GetRequiredService<CommandHandler>().InitializeAsync();
            // Bot token can be provided from the Configuration object we set up earlier
            await client.LoginAsync(TokenType.Bot, configuration["Token"]);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        public static async Task LogAsync(LogMessage message)
        {
            var severity = message.Severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            
            Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
            await Task.CompletedTask;
        }
        
        static ServiceProvider ConfigureServices (DiscordSocketConfig configuration)
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(configuration))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton(x => new CommandService())
                .AddSingleton<CommandHandler>()
                .AddSingleton<ClientHandler>()
                .AddSingleton<Main>()
                
                .BuildServiceProvider();
        }

        static bool IsDebug ( )
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }
    }
}