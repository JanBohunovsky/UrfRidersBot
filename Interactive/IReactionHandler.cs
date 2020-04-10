using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace UrfRiders.Interactive
{
    public interface IReactionHandler
    {
        RunMode RunMode { get; }

        Task ReactionAdded(IUserMessage message, IUser user, IEmote emote);
        Task ReactionRemoved(IUserMessage message, IUser user, IEmote emote);
    }
}