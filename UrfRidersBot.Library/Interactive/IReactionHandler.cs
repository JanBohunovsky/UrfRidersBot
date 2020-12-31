using System;
using System.Threading.Tasks;
using Discord;

namespace UrfRidersBot.Library
{
    public interface IReactionHandler
    {
        Task ReactionAdded(IUserMessage message, IUser user, IEmote emote);
        Task ReactionRemoved(IUserMessage message, IUser user, IEmote emote);
    }
}