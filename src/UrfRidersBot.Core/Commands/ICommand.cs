using System.Threading.Tasks;

namespace UrfRidersBot.Core.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }

        Task HandleAsync(InteractionContext context);
    }
    
    public interface ICommand<TGroup> : ICommand where TGroup : ICommandGroup
    {
    }
}