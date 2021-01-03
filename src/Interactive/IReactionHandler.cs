using System.Threading.Tasks;
using Discord;

namespace UrfRidersBot.Interactive
{
    public interface IReactionHandler
    {
        Task ReactionAdded(IUserMessage message, IUser user, IEmote emote);
        Task ReactionRemoved(IUserMessage message, IUser user, IEmote emote);
    }
}