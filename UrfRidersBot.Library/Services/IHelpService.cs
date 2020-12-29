using System.Threading.Tasks;
using Discord;

namespace UrfRidersBot.Library
{
    public interface IHelpService
    {
        /// <summary>
        /// Returns a detailed command with its description and usage.
        /// <para>
        /// If <see cref="commandName"/> is not found, then it returns an error embed.
        /// </para>
        /// </summary>
        /// <param name="context">Command context is required to filter out the commands that the user can execute.</param>
        /// <param name="commandName">Name of a command you want more details for.</param>
        ValueTask<Embed> GetCommandDetails(UrfRidersContext context, string commandName);
        
        /// <summary>
        /// Returns a list of available commands for current user.
        /// </summary>
        /// <param name="context">Command context is required to filter out the commands that the user can execute.</param>
        ValueTask<Embed> GetAllCommands(UrfRidersContext context);
    }
}