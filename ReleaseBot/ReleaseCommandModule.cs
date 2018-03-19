using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;
using Discord;
using Services;
using Services.Domain;

namespace ReleaseBot
{
    public class ReleaseCommandModule : ModuleBase<SocketCommandContext>
    {
        //Esta clase se llama desde cada servidor, es decir, si la llamo desde el server A,
        //el contexto es diferente de si la llamo desde el server B
        
        // ~say hello -> hello
        [Command("say")]
        [Summary("Echoes a message.")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            await ReplyAsync(echo);
        }
        [Command("init")]
        [Summary("Starts notifying when new releases pop.")]
        public async Task SubscribeToAll()
        {
            await ReplyAsync(Beautify("Checking server for new releases...\n"+
                "This process is automatic and will be repeated every hour"));
            SubscriptionService.SubscribeToAllSources("" + Context.Guild.Id);
            //The backend eventually will be in a different component, running in parallel
            BackendRunner.RunBackend();
        }

        private static string Beautify(string message)
        {
            return "```" + message + "```";
        }
    }
}
