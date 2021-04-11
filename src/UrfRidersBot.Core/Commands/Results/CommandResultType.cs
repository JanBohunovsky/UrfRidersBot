namespace UrfRidersBot.Core.Commands
{
    public enum CommandResultType
    {
        /// <summary>
        /// Command handled the response and nothing more should happen.
        /// </summary>
        NoAction,
        
        /// <summary>
        /// Command completed successfully and wants to send a success message.
        /// </summary>
        Success,
        
        /// <summary>
        /// Command wants to send an error message saying that invoking the command in current context is invalid.
        /// </summary>
        InvalidOperation,
        
        /// <summary>
        /// Command wants to send an error message saying that the user fucked up some parameters.
        /// </summary>
        InvalidParameter
    }
}