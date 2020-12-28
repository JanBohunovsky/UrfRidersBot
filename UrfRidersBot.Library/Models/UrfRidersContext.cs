using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UrfRidersBot.Library
{
    public class UrfRidersContext : SocketCommandContext
    {
        public string Prefix { get; set; }
        
        public UrfRidersContext(DiscordSocketClient client, SocketUserMessage msg, string prefix) : base(client, msg)
        {
            Prefix = prefix;
        }
    }
}