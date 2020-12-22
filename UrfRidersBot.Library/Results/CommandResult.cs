using Discord.Commands;

namespace UrfRidersBot.Library
{
    public class CommandResult : RuntimeResult
    {
        public CommandResultType ResultType { get; set; }

        public string? Title { get; set; }
        public bool DeleteUserMessage { get; set; }

        private CommandResult(CommandResultType type, CommandError? error, string? reason, string? title, bool deleteUserMessage = false) : base(error, reason)
        {
            ResultType = type;
            Title = title;
            DeleteUserMessage = deleteUserMessage;
        }

        public static CommandResult FromError(string reason, bool deleteUserMessage = false) => 
            new CommandResult(CommandResultType.Error, CommandError.Unsuccessful, reason, null, deleteUserMessage);
        public static CommandResult FromSuccess(string? message, bool deleteUserMessage = false) => 
            new CommandResult(CommandResultType.Success, null, message, null, deleteUserMessage);
        public static CommandResult FromInformation(string? message, string? title, bool deleteUserMessage = false) => 
            new CommandResult(CommandResultType.Information, null, message, title, deleteUserMessage);

        public static CommandResult FromEmpty(bool deleteUserMessage = false) =>
            new CommandResult(CommandResultType.Empty, null, null, null, deleteUserMessage);
    }

    public enum CommandResultType
    {
        Error,
        Success,
        Information,
        Empty,
    }
}