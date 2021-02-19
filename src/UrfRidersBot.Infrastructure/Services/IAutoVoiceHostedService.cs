using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;

namespace UrfRidersBot.Infrastructure
{
    public interface IAutoVoiceHostedService : IHostedService
    {
        IAsyncEnumerable<DiscordChannel> GetChannels(DiscordGuild guild);
        ValueTask<DiscordChannel> Enable(DiscordGuild guild, DiscordChannel? category = null);
        ValueTask<int> Disable(DiscordGuild guild);
    }
}