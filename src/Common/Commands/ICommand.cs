using System.Threading.Tasks;

namespace UrfRidersBot.Common.Commands
{
    public interface ICommand
    {
        Task HandleAsync(CommandContext context);
    }
}