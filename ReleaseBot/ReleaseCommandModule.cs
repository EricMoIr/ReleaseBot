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
            string guildId = "" + Context.Guild.Id;
            //Not sure if updating context is needed. Needs testing
            if (Run.contexts.ContainsKey(guildId))
            {
                Run.contexts.Remove(guildId);
            }
            else
            {
                Run.newReleases.Add(guildId, new List<ReleaseView>());
            }
            Run.contexts.Add(guildId, Context);
            await ReplyAsync("The channel <#" + Context.Channel.Id + "> was set for notifications");
        }
        [Command("sub")]
        [Summary("Starts notifying when new releases subscribed pop. Use: '.sub all' or '.sub <source>' or '.sub <category>'")]
        public async Task Subscribe(string arg = null)
        {
            if (arg == null)
            {
                ReplyAsync("The correct use of this command is '.sub all' or '.sub <source>' or '.sub <category>'");
            }
            else if (arg == "all")
            {
                ReplyAsync("Subscribing to all sources...\n" +
                        "You will receive a message every hour notifying about the sources you are subscribed to");
                SubscriptionService.SubscribeToAllSources("" + Context.Guild.Id);
            }
            else {
                string category = SourceService.FindCategory(arg);
                string source = SourceService.FindSource(arg);
                if (category != null)
                {
                    ReplyAsync($"Subscribing to {category}\n" +
                        "You will receive a message every hour notifying about the sources you are subscribed to");
                    SubscriptionService.SubscribeToCategory(category, "" + Context.Guild.Id);
                }
                else if (source != null)
                {
                    ReplyAsync("Subscribing to " + SourceView.CleanURL(source) + "\n" +
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
                ReplyAsync("The correct use of this command is '.unsub all' or '.unsub <source>' or '.unsub <category>'");
            }
            else if (arg == "all")
            {
                ReplyAsync("Unsubscribing from all sources...");
                SubscriptionService.UnsubscribeFromAllSources("" + Context.Guild.Id);
            }
            else {
                string category = SourceService.FindCategory(arg);
                string source = SourceService.FindSource(arg);
                if (category != null)
                {
                    ReplyAsync("Unsubscribing from " + category);
                    SubscriptionService.UnsubscribeFromCategory(category, "" + Context.Guild.Id);
                }
                else if (source != null)
                {
                    ReplyAsync("Unsubscribing from " + SourceView.CleanURL(source));
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
            List<SourceView> sources = SubscriptionService.GetAll(""+Context.Guild.Id);
            Printer.PrintSubscriptions(sources, Context.Channel);
        }


        //Categories should be a thing later on
        [Command("sources")]
        [Summary("Prints available sources. Use: '.sources all' or '.sources <category>'")]
        public async Task PrintSources(string arg = null)
        {
            if (arg == null)
            {
                ReplyAsync("The correct use of this command is '.sources all' or '.sources <category>'");
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
                await Run.NotifyServer(Context, Run.INTERVAL);
            }
            else
            {
                int hours;
                if (int.TryParse(interval, out hours) && hours > 0 && hours <= 10)
                {
                    await Run.NotifyServer(Context, hours * 3600000);
                }
                else
                {
                    await ReplyAsync("The correct use of this command is '.releases <hours>'. <hours> is optional and its minimum is 1 hour and maximum 10 hours");
                }
            }
        }

        internal static bool CanAddToMessage(StringBuilder message, StringBuilder inner)
        {
            return message.Length + inner.Length < 1000;
        }
    }
}
