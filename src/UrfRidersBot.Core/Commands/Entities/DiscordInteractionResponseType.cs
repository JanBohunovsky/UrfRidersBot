namespace UrfRidersBot.Core.Commands.Entities
{
    public enum DiscordInteractionResponseType
    {
        /// <summary>
        /// ACK a Ping.
        /// </summary>
        Pong = 1,
        
        /// <summary>
        /// Respond to an interaction with a message.
        /// </summary>
        ChannelMessageWithSource = 4,
        
        /// <summary>
        /// ACK an interaction and edit to a response later, the user sees a loading state.
        /// </summary>
        DeferredChannelMessageWithSource = 5
    }
}