using Discord;
using Discord.WebSocket;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseBot
{
    class Printer
    {
        internal static readonly string CATEGORY_NOT_FOUND = "The category could not be found.";

        public static async Task PrintSubscriptions(List<SourceView> sources, ISocketMessageChannel channel)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("You have ").Append(sources.Count).AppendLine(" subscriptions");
            foreach (SourceView source in sources)
            {
                sb.Append("- ").AppendLine(source.URL);
            }
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = "Subscriptions";
            builder.Description = sb.ToString();
            await channel.SendMessageAsync("", embed: builder.Build());
        }

        internal static async Task PrintSources(IEnumerable<IGrouping<string, SourceView>> sourcesByCategory,
            ISocketMessageChannel channel, string category = "")
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = "Sources";
            if(category == null)
            {
                builder.AddField(f =>
                {
                    f.Name = "Wrong category";
                    f.Value = "The category used as parameters doesn't exist";
                });
            }
            else
            {
                foreach (var categoryGroup in sourcesByCategory)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var source in categoryGroup)
                    {
                        sb.AppendLine("- " + source.URL);
                    }
                    builder.AddField(f =>
                    {
                        f.Name = categoryGroup.Key;
                        f.Value = sb.ToString();
                    });
                }
            }
            await channel.SendMessageAsync("", embed: builder.Build());
        }

        internal static async Task PrintError(string error, ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync(error);
        }
    }
}
