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

            // Subscribe the logging handler to both the client and the CommandService.
            _client.Log += Log;
            _commands.Log += Log;

            // Setup your DI container.

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
        private double INTERVAL = 3600000; //one hour
        Dictionary<string, Tuple<SocketCommandContext, List<ReleaseView>>> releasesOfServers
            = new Dictionary<string, Tuple<SocketCommandContext, List<ReleaseView>>>();
        private async Task MainAsync()
        {
            // Centralize the logic for commands into a separate method.
            await InitCommands();

            // Login and connect.
            await _client.LoginAsync(TokenType.Bot, "MjYyNzMxMzg4NjY1NDYyNzg1.DX48DQ.u6vJgafC628dfIWJ_3F8hWvBeuY");
            await _client.StartAsync();

            System.Timers.Timer checkForTime = new System.Timers.Timer(INTERVAL);
            NotifyServers();
            checkForTime.Elapsed += new ElapsedEventHandler(NotifyServersEvent);
            checkForTime.Enabled = true;

            // Wait infinitely so your bot actually stays connected.
            await Task.Delay(Timeout.Infinite);
        }

        private void NotifyServersEvent(object sender, ElapsedEventArgs e)
        {
            NotifyServers();
        }

        private void NotifyServers()
        {
            foreach(string serverId in releasesOfServers.Keys)
            {
                List<ReleaseView> releases = ReleaseService.Get(serverId);
            }
            foreach(Tuple<SocketCommandContext, List<ReleaseView>> entry in releasesOfServers.Values)
            {
                PrintReleases(entry.Item2, entry.Item1);
            }
        }

        private void PrintReleases(List<ReleaseView> releases, SocketCommandContext context)
        {
            StringBuilder message = new StringBuilder();
            message.Append(releases.Count).Append(" new releases were found").AppendLine();
            foreach (ReleaseView release in releases)
            {
                message.Append("- ").Append(release.Name)
                    .Append(" ").Append(release.Chapter)
                    .Append(" (").Append(release.SourceURL).Append(")")
                    .AppendLine();
            }
            context.Channel.SendMessageAsync(Beautify(message.ToString()));
        }

        private static string Beautify(string message)
        {
            return "```" + message + "```";
        }

        private async Task InitCommands()
        {
            // Subscribe a handler to see if a message invokes a command.
            await _commands.AddModuleAsync<ReleaseCommandModule>();
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

                //Not sure if updating context is needed. Needs testing
                Tuple<SocketCommandContext, List<ReleaseView>> value;
                var hasServer = releasesOfServers.TryGetValue("" + context.Guild.Id, out value);
                if (hasServer)
                {
                    //update stored context
                    value = new Tuple<SocketCommandContext, List<ReleaseView>>(context, value.Item2);
                }
                else
                {
                    value = new Tuple<SocketCommandContext, List<ReleaseView>>(context, new List<ReleaseView>());
                }
                releasesOfServers.Add("" + context.Guild.Id, value);


                var result = await _commands.ExecuteAsync(context, pos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
