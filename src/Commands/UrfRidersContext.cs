using Discord.Commands;
using Discord.WebSocket;

namespace UrfRidersBot
{
    public class UrfRidersContext : SocketCommandContext
    {
        public string Prefix { get; }
        
        public UrfRidersContext(DiscordSocketClient client, SocketUserMessage msg, string prefix) : base(client, msg)
        {
            Prefix = prefix;
        }
    }
}