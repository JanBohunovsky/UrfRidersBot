using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace UrfRiders.Modules.Interactive
{
    public interface IReactionHandler
    {
        RunMode RunMode { get; }

        Task ReactionAdded(IUserMessage message, IUser user, IEmote emote);
        Task ReactionRemoved(IUserMessage message, IUser user, IEmote emote);
    }
}