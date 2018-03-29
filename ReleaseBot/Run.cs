using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Services.Domain;
using System.Text;
using Services;
using System.Linq;
using System.Configuration;

namespace ReleaseBot
{
    class Run
    {
        static void Main(string[] args)
        {
            // Call the Program constructor, followed by the 
            // MainAsync method and wait until it finishes (which should be never).
            new Run().MainAsync().GetAwaiter().GetResult();
        }

        private readonly DiscordSocketClient _client;

        // Keep the CommandService and DI container around for use with commands.
        // These two types require you install the Discord.Net.Commands package.
        private readonly CommandService _commands;

        private Run()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                // How much logging do you want to see?
                LogLevel = LogSeverity.Info,

                // If you or another service needs to do anything with messages
                // (eg. checking Reactions, checking the content of edited/deleted messages),
                // you must set the MessageCacheSize. You may adjust the number as needed.
                //MessageCacheSize = 50,

                // If your platform doesn't have native websockets,
                // add Discord.Net.Providers.WS4Net from NuGet,
                // add the `using` at the top, and uncomment this line:
                //WebSocketProvider = WS4NetProvider.Instance
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
            });
            
            _client.Log += Log;
            _commands.Log += Log;
            
            _client.JoinedGuild += JoinedNewGuild;
            _client.SetGameAsync("Use .help for commands");
        }

        private Task JoinedNewGuild(SocketGuild arg)
        {
            string message = "Thank you for adding <@" + _client.CurrentUser.Id + "> to your server!";
            message += "\nThe best solution for subscribing to sites' publications :D";
            message += "\nTo set in which channel you wish to receive the notifications, enter '.setchannel' at that channel";
            message += "\nFor a detailed information concerning the commands, enter '.help'";
            message += "\nEnjoy! :D";
            arg.DefaultChannel.SendMessageAsync(message);
            return Task.CompletedTask;
        }

        // Example of a logging handler. This can be re-used by addons
        // that ask for a Func<LogMessage, Task>.
        private static Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        }
        internal static double INTERVAL = 3600000; //one hour
        //private static double INTERVAL = 60000; //one minute
        internal static Dictionary<string, SocketCommandContext> contexts
            = new Dictionary<string, SocketCommandContext>();
        internal static Dictionary<string, List<ReleaseView>> newReleases
            = new Dictionary<string, List<ReleaseView>>();
        internal static System.Timers.Timer checkForTime = new System.Timers.Timer(INTERVAL);
        private async Task MainAsync()
        {
            // Centralize the logic for commands into a separate method.
            await InitCommands();

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["BotToken"]);
            await _client.StartAsync();

            //The backend eventually will start running independently of the bot
            BackendRunner.RunBackend(INTERVAL/3);

            checkForTime.Elapsed += new ElapsedEventHandler(NotifyServersEvent);
            checkForTime.Start();

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }

        private void NotifyServersEvent(object sender, ElapsedEventArgs e)
        {
            NotifyServers();
        }

        private void NotifyServers()
        {
            foreach (string serverId in contexts.Keys)
            {
                SocketCommandContext context;
                if (contexts.TryGetValue(serverId, out context))
                    NotifyServer(context, INTERVAL);
            }
        }
        internal static async Task NotifyServer(SocketCommandContext context, double interval)
        {
            string serverId = "" + context.Guild.Id;
            List<ReleaseView> releases = ReleaseService.GetNewReleasesOfServer(serverId, interval);
            List<ReleaseView> newReleasesOfServer;
            List<ReleaseView> toPrint = new List<ReleaseView>();
            if (newReleases.TryGetValue(serverId, out newReleasesOfServer))
            {
                for (int i = 0; i < releases.Count; i++)
                {
                    if (!newReleasesOfServer.Contains(releases[i]))
                    {
                        ReleaseView release = toPrint.Find(x => x.Equals(releases[i]));
                        if (release != null)
                            release.Sources.AddRange(releases[i].Sources);
                        else
                            toPrint.Add(releases[i]);
                    }
                }
                newReleasesOfServer.AddRange(toPrint);
                newReleases.Remove(serverId);
                newReleases.Add(serverId, newReleasesOfServer);
                await PrintReleases(toPrint, context);
            }
            else
            {
                await PrintReleases(releases, context);
            }
        }

        private static async Task PrintReleases(List<ReleaseView> releases, SocketCommandContext context)
        {
            EmbedBuilder builder = new EmbedBuilder();
            StringBuilder message = new StringBuilder();
            builder.Title = releases.Count + " new releases were found";
            int releaseNumber = 0;
            foreach (ReleaseView release in releases)
            {
                IEnumerable<string> sourceURLs = release.Sources.Select(x => x.URL);
                StringBuilder inner = new StringBuilder();
                inner.Append("- ").Append(release.Name)
                    .Append(" ").Append((release.Chapter == 0)? "": ""+release.Chapter)
                    .Append(" (").Append(string.Join(" - ", sourceURLs)).Append(")")
                    .AppendLine();
                if (ReleaseCommandModule.CanAddToMessage(message, inner) && releaseNumber++ < 15)
                    message.Append(inner);
                else
                    break;
            }
            builder.Description = message.ToString();
            await context.Channel.SendMessageAsync("", embed: builder.Build());
        }

        private async Task InitCommands()
        {
            // Subscribe a handler to see if a message invokes a command.
            await _commands.AddModuleAsync<ReleaseCommandModule>();
            await _commands.AddModuleAsync<HelpCommandModule>();
            HelpCommandModule._commands = _commands;

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            // We don't want the bot to respond to itself or other bots.
            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            int pos = 0;

            if (msg.HasCharPrefix('.', ref pos))
            {
                // Create a Command Context.
                var context = new SocketCommandContext(_client, msg);

                var result = await _commands.ExecuteAsync(context, pos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
