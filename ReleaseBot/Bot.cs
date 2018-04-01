using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseBot
{
    class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        internal Dictionary<string, SocketCommandContext> Contexts { get; private set; }
        internal Dictionary<string, List<ReleaseView>> NewReleases { get; private set; }
        public bool IsRunning { get; private set; }

        internal Bot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
            });

            _client.Log += Log;
            _commands.Log += Log;

            _client.JoinedGuild += JoinedNewGuild;
            _client.SetGameAsync("Use .help");

            Contexts = new Dictionary<string, SocketCommandContext>();
            NewReleases = new Dictionary<string, List<ReleaseView>>();
        }
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

        private Task JoinedNewGuild(SocketGuild arg)
        {
            StringBuilder message = new StringBuilder("Thank you for adding <@").Append(_client.CurrentUser.Id).AppendLine("> to your server!");
            message.AppendLine("The best solution for subscribing to sites' publications :D");
            message.AppendLine("To set in which channel you wish to receive the notifications, enter '.setchannel' at that channel");
            message.AppendLine("For a detailed information concerning the commands, enter '.help'");
            message.AppendLine("Enjoy! :D");
            arg.DefaultChannel.SendMessageAsync(message.ToString());
            return Task.CompletedTask;
        }
        internal async Task ConnectAsync()
        {
            await InitCommands();

            await _client.LoginAsync(TokenType.Bot, ConfigurationManager.AppSettings["BotToken"]);
            await _client.StartAsync();
            IsRunning = true;
        }

        private async Task InitCommands()
        {
            await _commands.AddModuleAsync<ReleaseCommandModule>();
            await _commands.AddModuleAsync<HelpCommandModule>();
            HelpCommandModule._commands = _commands;

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

            int pos = 0;

            if (msg.HasCharPrefix('.', ref pos))
            {
                var context = new SocketCommandContext(_client, msg);

                var result = await _commands.ExecuteAsync(context, pos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
