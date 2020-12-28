using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace UrfRidersBot.Library
{
    public class UrfRidersContext : SocketCommandContext
    {
        public UrfRidersContext(DiscordSocketClient client, SocketUserMessage msg) : base(client, msg)
        {
            
        }
    }
}