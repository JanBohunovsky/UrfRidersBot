using Discord;

namespace UrfRidersBot.Library
{
    public class GuildData
    {
        public string Id => GetId(GuildId);
        public ulong GuildId { get; internal set; }
        public string? RandomValue { get; set; }
        
        private GuildData() {}

        public static GuildData FromGuild(IGuild guild) => new GuildData { GuildId = guild.Id };

        public static string GetId(ulong guildId)=> $"GuildData/{guildId}";
        
        public static string GetId(IGuild guild) => GetId(guild.Id);
    }
}