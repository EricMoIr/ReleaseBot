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
        //Esta clase se llama desde cada servidor, es decir, si la llamo desde el server A,
        //el contexto es diferente de si la llamo desde el server B
        // ~say hello -> hello
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
            await ReplyAsync("The channel <#" + Context.Channel.Id+"> was set for notifications");
        }
        [Command("sub")]
        [Summary("Starts notifying when new releases subscribed pop. Use: '.sub all' or '.sub <source>' or '.sub <category>'")]
        public async Task SubscribeToAll(string arg)
        {
            if (arg == null)
            {
                ReplyAsync("The correct use of this command is '.sub all' or '.sub <source>' or '.sub <category>'");
            }
            else if(arg == "all")
            {
                ReplyAsync("Subscribing to all sources...\n" +
                    "You will receive a message every hour notifying about the sources you are subscribed to");
                SubscriptionService.SubscribeToAllSources("" + Context.Guild.Id);
            }
            else if (SourceService.IsValidCategory(ref arg))
            {
                ReplyAsync("Subscribing to " + arg + "\n" +
                    "You will receive a message every hour notifying about the sources you are subscribed to");
                SubscriptionService.SubscribeToCategory(arg, "" + Context.Guild.Id);
            }
            else if(SourceService.IsValidSource(ref arg))
            {
                ReplyAsync("Subscribing to "+arg+"\n" +
                    "You will receive a message every hour notifying about the sources you are subscribed to");
                SubscriptionService.SubscribeToSource(arg, "" + Context.Guild.Id);
            }
            else
            {
                await ReplyAsync("You must choose one of the pre-existing sources");
                await PrintSources("all");
            }
        }

        //Categories should be a thing later on
        [Command("sources")]
        [Summary("Prints available sources. Use: '.sources all' or '.sources <category>'")]
        public async Task PrintSources(string arg)
        {
            if (arg == null)
            {
                ReplyAsync("The correct use of this command is '.sources all' or '.sources <category>'");
            }
            else
            {
                EmbedBuilder builder = new EmbedBuilder();
                builder.Title = "Sources";
                var sourcesByCategory = SourceService.GetAllViews().GroupBy(x => x.Category);
                if (arg == "all")
                {
                    foreach (var categoryGroup in sourcesByCategory)
                    {
                        string category = categoryGroup.Key;
                        StringBuilder sb = new StringBuilder();
                        foreach (var source in categoryGroup)
                        {
                            sb.AppendLine("- " + source.URL);
                        }
                        builder.AddField(f =>
                        {
                            f.Name = category;
                            f.Value = sb.ToString();
                        });
                    }
                    await ReplyAsync("", embed: builder.Build());
                }
                else
                {//copypaste is bad mmmkay
                    foreach (var categoryGroup in sourcesByCategory)
                    {
                        string category = categoryGroup.Key;
                        if (arg.Trim().ToLower() != category.Trim().ToLower()) continue;
                        StringBuilder sb = new StringBuilder();
                        foreach (var source in categoryGroup)
                        {
                            sb.AppendLine("- " + source.URL);
                        }
                        builder.AddField(f =>
                        {
                            f.Name = category;
                            f.Value = sb.ToString();
                        });
                    }
                    if (builder.Fields.Count == 0)
                        builder.AddField(f =>
                        {
                            f.Name = "Wrong category";
                            f.Value = "The category used as parameters doesn't exist";
                        });
                    await ReplyAsync("", embed: builder.Build());
                }
            }
        }
        
        [Command("releases")]
        [Summary("Prints the last found releases.")]
        public async Task PrintReleases()
        {
            await Run.NotifyServer(Context);
        }

        internal static bool CanAddToMessage(StringBuilder message, StringBuilder inner)
        {
            return message.Length + inner.Length < 1000;
        }
    }
}
