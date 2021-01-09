using System.Threading.Tasks;
using DSharpPlus;

namespace UrfRidersBot
{
    public interface IOnReadyService
    {
        Task OnReady(DiscordClient client);
    }
}