using System.Threading.Tasks;

namespace UrfRidersBot.Core.Commands
{
    public class CommandResult
    {
        public CommandResultType Type { get; }
        public string? Message { get; }
        
        private CommandResult(CommandResultType type, string? message = null)
        {
            Type = type;
            Message = message;
        }

        /// <summary>
        /// Indicates that the command handled the response and nothing more should happen.
        /// </summary>
        public static CommandResult NoAction => new(CommandResultType.NoAction);

        /// <summary>
        /// Success message for the user.
        /// </summary>
        public static CommandResult Success(string? message = null)
            => new(CommandResultType.Success, message);

        /// <summary>
        /// Error message for the user saying that invoking the command in current context is invalid.
        /// <para>
        /// Example: Enabling a module while it's already enabled.
        /// </para>
        /// </summary>
        public static CommandResult InvalidOperation(string message)
            => new(CommandResultType.InvalidOperation, message);

        /// <summary>
        /// Error message for the user saying that he fucked up some parameters.
        /// <para>
        /// Example: Color format is #RRGGBB but user wrote something else.
        /// </para>
        /// </summary>
        public static CommandResult InvalidParameter(string message) 
            => new(CommandResultType.InvalidParameter, message);
        
        public static implicit operator ValueTask<CommandResult>(CommandResult result) => new(result);
    }
}