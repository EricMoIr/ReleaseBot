using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;

namespace ReleaseBot
{
    public class ReleaseCommandModule : ModuleBase<SocketCommandContext>
    {
        // ~say hello -> hello
        [Command("say")]
        [Summary("Echoes a message.")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            await ReplyAsync(echo);
        }
    }
}
