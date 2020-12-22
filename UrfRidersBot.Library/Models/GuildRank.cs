namespace UrfRidersBot.Library
{
    public enum GuildRank
    {
        /// <summary>
        /// Literally everyone.
        /// </summary>
        Everyone,
        
        /// <summary>
        /// Usually a more trusted or verified user in the guild.
        /// <para>
        /// If guild didn't specify a role for this rank, then we assume that everyone is a member.
        /// </para>
        /// </summary>
        Member,
        
        /// <summary>
        /// Usually users that can manage channels, messages and stuff, literally moderate the guild.
        /// <para>
        /// If guild didn't specify a role for this rank, then we assume that everyone that has "Manage Channels" permission is a moderator.
        /// </para>
        /// </summary>
        Moderator,
        
        /// <summary>
        /// Usually the guild owners.
        /// <para>
        /// If guild didn't specify a role for this rank, then we assume that only the guild owner is an admin.
        /// </para>
        /// <para>
        /// Note: This rank gives them the ability to configure and control this bot.
        /// </para>
        /// </summary>
        Admin,
        
        /// <summary>
        /// Owner of the guild.
        /// </summary>
        Owner
    }
}