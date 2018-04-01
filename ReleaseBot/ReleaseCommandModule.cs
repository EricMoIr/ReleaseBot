using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;
using Discord;
using Services;
using System.Text.RegularExpressions;
using Services.Domain;

namespace ReleaseBot
{
    public class ReleaseCommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("setchannel")]
        [Summary("Sets this channel as the one to print notifications.")]
        public async Task SetChannel()
        {
            BotService.AddGuild(Context);
            await ReplyAsync("The channel <#" + Context.Channel.Id + "> was set for notifications");
        }
        [Command("sub")]
        [Summary("Starts notifying when new releases subscribed pop. Use: '.sub all' or '.sub <source>' or '.sub <category>'")]
        public async Task Subscribe(string arg = null)
        {
            if (arg == null)
            {
                await ReplyAsync("The correct use of this command is '.sub all' or '.sub <source>' or '.sub <category>'");
            }
            else if (arg == "all")
            {
                await ReplyAsync("Subscribing to all sources...\n" +
                        "You will receive a message every hour notifying about the sources you are subscribed to");
                SubscriptionService.SubscribeToAllSources("" + Context.Guild.Id);
            }
            else {
                string category = SourceService.FindCategory(arg);
                string source = SourceService.FindSource(arg);
                if (category != null)
                {
                    await ReplyAsync($"Subscribing to {category}\n" +
                        "You will receive a message every hour notifying about the sources you are subscribed to");
                    SubscriptionService.SubscribeToCategory(category, "" + Context.Guild.Id);
                }
                else if (source != null)
                {
                    await ReplyAsync("Subscribing to " + SourceView.CleanURL(source) + "\n" +
                        "You will receive a message every hour notifying about the sources you are subscribed to");
                    SubscriptionService.SubscribeToSource(source, "" + Context.Guild.Id);
                }
                else
                {
                    await ReplyAsync("You must choose one of the pre-existing sources/categories");
                    await PrintSources("all");
                }
            }
        }
        [Command("unsub")]
        [Summary("Removes the selected subscription. Use: '.unsub all' or '.unsub <source>' or '.unsub <category>'")]
        public async Task Unsubscribe(string arg = null)
        {
            if (arg == null)
            {
                await ReplyAsync("The correct use of this command is '.unsub all' or '.unsub <source>' or '.unsub <category>'");
            }
            else if (arg == "all")
            {
                await ReplyAsync("Unsubscribing from all sources...");
                SubscriptionService.UnsubscribeFromAllSources("" + Context.Guild.Id);
            }
            else {
                string category = SourceService.FindCategory(arg);
                string source = SourceService.FindSource(arg);
                if (category != null)
                {
                    await ReplyAsync("Unsubscribing from " + category);
                    SubscriptionService.UnsubscribeFromCategory(category, "" + Context.Guild.Id);
                }
                else if (source != null)
                {
                    await ReplyAsync("Unsubscribing from " + SourceView.CleanURL(source));
                    SubscriptionService.UnsubscribeFromSource(source, "" + Context.Guild.Id);
                }
                else
                {
                    await ReplyAsync("You must choose one of the pre-existing sources/categories");
                    await PrintSources("all");
                }
            }
        }

        [Command("subs")]
        [Summary("Prints all your subscriptions. Use: '.subs'")]
        public async Task PrintSubscriptions()
        {
            List<SourceView> sources = SubscriptionService.GetAll("" + Context.Guild.Id);
            await Printer.PrintSubscriptions(sources, Context.Channel);
        }


        //Categories should be a thing later on
        [Command("sources")]
        [Summary("Prints available sources. Use: '.sources all' or '.sources <category>'")]
        public async Task PrintSources(string arg = null)
        {
            if (arg == null)
            {
                await ReplyAsync("The correct use of this command is '.sources all' or '.sources <category>'");
            }
            else
            {
                if (arg == "all")
                {
                    var sourcesByCategory = SourceService.GetAllViews().GroupBy(x => x.Category);
                    await Printer.PrintSources(sourcesByCategory, Context.Channel);
                }
                else
                {
                    string category = SourceService.FindCategory(arg);
                    if (category == null)
                    {
                        await Printer.PrintError(Printer.CATEGORY_NOT_FOUND, Context.Channel);
                    }
                    else
                    {
                        var sourcesByCategory = SourceService.GetAllViews()
                            .Where(x => x.Category == category)
                            .GroupBy(x => x.Category);
                        await Printer.PrintSources(sourcesByCategory, Context.Channel);
                    }
                }
            }
        }

        [Command("releases")]
        [Summary("Prints the last found releases. Use: '.releases'")]
        public async Task PrintReleases(string interval = null)
        {
            if (interval == null)
            {
                await BotService.NotifyServer(Context);
            }
            else
            {
                int hours;
                if (int.TryParse(interval, out hours) && hours > 0 && hours <= 10)
                {
                    await BotService.NotifyServerWithRepeated(Context, hours * 3600000);
                }
                else
                {
                    await ReplyAsync("The correct use of this command is '.releases <hours>'. <hours> is optional and its minimum is 1 hour and maximum 10 hours");
                }
            }
        }

        [Command("stalk")]
        [Summary("Prints information about a given user. Use: '.stalk <user_name> <source>'")]
        public async Task PrintStats(string username, string sourceURL)
        {
            User user = ReleaseService.GetUser(username, sourceURL);
            if (user == null)
            {
                await ReplyAsync("I couldn't find any information about that user in our database :cold_sweat:");
            }
            else
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.Title = "Stats of " + user.Name;

                foreach (SourceView source in user.SourcesWrittenAt())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Reported posts: ").AppendLine("" + user.PostsAt(source))
                        .Append("Thread with highest activity: ").AppendLine(user.MostUsedThread(source))
                        .Append("Day with highest activity: ").AppendLine(IntToDay(user.BusiestDay(source)))
                        .Append("Day with least activity: ").AppendLine(IntToDay(user.LeastBusyDay(source)))
                        .AppendLine("Times with least activity recorded:");
                    var offlineTimes = user.OfflineTimes(source);
                    for (int i = 0; i < 7; i++)
                    {
                        string day = IntToDay(i);
                        sb.Append(day).AppendLine(": ");
                        if (offlineTimes[i].Count == 0)
                        {
                            sb.AppendLine("No inactivity");
                            continue;
                        }
                        int min = offlineTimes[i][0];
                        int max = min + 1;

                        sb.Append(min).Append(" - ");

                        for (int j = 1; j < offlineTimes[i].Count; j++)
                        {
                            if (offlineTimes[i][j] == max)
                            {
                                max++;
                                continue;
                            }
                            min = offlineTimes[i][j];
                            sb.AppendLine("" + max).Append(min).Append(" - ");
                            max += 2;
                        }
                        sb.AppendLine("" + max);
                    }
                    builder.AddField(f =>
                    {
                        f.Name = source.URL;
                        f.Value = sb.ToString();
                    });
                }
                await ReplyAsync("", embed: builder.Build());
            }
        }

        private string IntToDay(int i)
        {
            switch (i)
            {
                case 1: return "Monday";
                case 2: return "Tuesday";
                case 3: return "Wednesday";
                case 4: return "Thursday";
                case 5: return "Friday";
                case 6: return "Saturday";
                default: return "Sunday";
            }
        }

        protected override Task<IUserMessage> ReplyAsync(string message, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            if (embed != null)
                return base.ReplyAsync(message, isTTS, embed, options);
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = "ReleaseBot";
            builder.Description = message;
            return base.ReplyAsync("", isTTS, builder.Build(), options);
        }

        internal static bool CanAddToMessage(StringBuilder message, StringBuilder inner)
        {
            return message.Length + inner.Length < 1000;
        }
    }
}
