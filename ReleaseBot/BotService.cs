using Discord;
using Discord.Commands;
using Services;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ReleaseBot
{
    class BotService
    {
        private static Bot bot;
        private const double INTERVAL = 3600000; //one hour
        //private const double INTERVAL = 60000; //one minute
        internal static async Task RunAsync()
        {
            if (bot != null)
                throw new InvalidOperationException("The bot is already running");
            bot = new Bot();

            System.Timers.Timer checkForTime = new System.Timers.Timer(INTERVAL);
            checkForTime.Elapsed += new ElapsedEventHandler(NotifyServersEvent);
            checkForTime.Start();

            await bot.ConnectAsync();
            BackendRunner.RunBackend(INTERVAL / 3);

            await Task.Delay(Timeout.Infinite);
        }

        internal static void AddGuild(SocketCommandContext context)
        {
            CheckBotExists();
            string guildId = "" + context.Guild.Id;
            //Not sure if updating context is needed. Needs testing
            if (bot.Contexts.ContainsKey(guildId))
            {
                bot.Contexts[guildId] = context;
            }
            else
            {
                bot.NewReleases.Add(guildId, new List<ReleaseView>());
                bot.Contexts.Add(guildId, context);
            }
        }
        private static void CheckBotExists()
        {
            if (bot == null || !bot.IsRunning)
                throw new InvalidOperationException("The bot is not running yet");
        }

        private static void NotifyServersEvent(object sender, ElapsedEventArgs e)
        {
            NotifyServers();
        }
        private static async void NotifyServers()
        {
            foreach (string serverId in bot.Contexts.Keys)
            {
                SocketCommandContext context;
                if (bot.Contexts.TryGetValue(serverId, out context))
                    await NotifyServer(context, INTERVAL);
            }
        }
        internal static async Task NotifyServer(SocketCommandContext context, double interval = INTERVAL)
        {
            CheckBotExists();
            string serverId = "" + context.Guild.Id;
            List<ReleaseView> releases = ReleaseService.GetNewReleasesOfServer(serverId, interval);
            List<ReleaseView> toPrint = new List<ReleaseView>();
            if (bot.NewReleases.ContainsKey(serverId))
            {
                List<ReleaseView> newReleasesOfServer = bot.NewReleases[serverId];
                for (int i = 0; i < releases.Count; i++)
                {
                    if (!newReleasesOfServer.Contains(releases[i]))
                    {
                        toPrint.Add(releases[i]);
                    }
                }
                bot.NewReleases[serverId].AddRange(toPrint);
                await PrintReleases(toPrint, context);
            }
            else
            {
                await PrintReleases(releases, context);
            }
        }

        internal static async Task NotifyServerWithRepeated(SocketCommandContext context, int interval)
        {
            CheckBotExists();
            string serverId = "" + context.Guild.Id;
            List<ReleaseView> releases = ReleaseService.GetNewReleasesOfServer(serverId, interval);
            await PrintReleases(releases, context);
        }

        private static async Task PrintReleases(List<ReleaseView> releases, SocketCommandContext context)
        {
            EmbedBuilder builder = new EmbedBuilder();
            StringBuilder message = new StringBuilder();

            var releasesBySource = releases.GroupBy(x => new { x.Name, x.Chapter, x.DatePublished });
            List<ReleaseView> toPrint = new List<ReleaseView>();
            List<List<string>> linksPerRelease = new List<List<string>>();
            foreach (var group in releasesBySource)
            {
                List<string> links = new List<string>();
                toPrint.Add(group.FirstOrDefault());
                foreach (var releasesOfSource in group)
                {
                    links.Add(releasesOfSource.Source.ToString());
                }
                linksPerRelease.Add(links);
            }

            builder.Title = toPrint.Count + " new releases were found";
            int releaseNumber = 0;
            int i = 0;
            foreach (ReleaseView release in releases)
            {
                StringBuilder inner = new StringBuilder();
                inner.Append("- ").Append(release.Name)
                    .Append(" ").Append((release.Chapter == 0) ? "" : "" + release.Chapter)
                    .Append(" (").Append(string.Join(" - ", linksPerRelease[i++])).Append(")")
                    .AppendLine();
                if (ReleaseCommandModule.CanAddToMessage(message, inner) && releaseNumber++ < 15)
                    message.Append(inner);
                else
                    break;
            }
            builder.Description = message.ToString();
            await context.Channel.SendMessageAsync("", embed: builder.Build());
        }
    }
}
