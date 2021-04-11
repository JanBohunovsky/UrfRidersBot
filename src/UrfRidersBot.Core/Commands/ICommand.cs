using System.Threading.Tasks;

namespace UrfRidersBot.Core.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Whether responses from this command will be ephemeral only or not.
        /// </summary>
        bool Ephemeral { get; }
        
        string Name { get; }
        string Description { get; }
        
        ValueTask<CommandResult> HandleAsync(ICommandContext context);
    }
    
    public interface ICommand<TParent> : ICommand where TParent : ICommandGroup
    {
    }
}