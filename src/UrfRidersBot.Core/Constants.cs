using DSharpPlus.Entities;

namespace UrfRidersBot.Core
{
    public static class UrfRidersColor
    {
        
        /// <summary>
        /// Discord's cyan, or #1ABC9C.
        /// </summary>
        public static readonly DiscordColor Cyan = new DiscordColor(0x1ABC9C);
        
        /// <summary>
        /// UrfRiders' blue, or #05B3EB.
        /// </summary>
        public static readonly DiscordColor Blue = new DiscordColor(0x05B3EB);
        
        /// <summary>
        /// Discord's green, or #2ECC71.
        /// </summary>
        public static readonly DiscordColor Green = new DiscordColor(0x2ECC71);
        
        /// <summary>
        /// Discord's yellow, or #F1C40F.
        /// </summary>
        public static readonly DiscordColor Yellow = new DiscordColor(0xF1C40F);
        
        /// <summary>
        /// Discord's red, or #E74C3C.
        /// </summary>
        public static readonly DiscordColor Red = new DiscordColor(0xE74C3C);
        
        /// <summary>
        /// Bot's theme color.
        /// </summary>
        public static readonly DiscordColor Theme = Cyan;
    }

    public static class UrfRidersIcon
    {
        public static readonly string Checkmark = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/checkmark.png";
        public static readonly string Ok = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/ok.png";
        public static readonly string Error = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/error.png";
        public static readonly string HighPriority = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/high_priority.png";
        public static readonly string Checked = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/checked.png";
        public static readonly string Cancel = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/cancel.png";
        public static readonly string Unavailable = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/unavailable.png";
        public static readonly string Delete = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/master/images/delete.png";
    }

    public static class UrfRidersEmotes
    {
        public static readonly string Checkmark = "<:checkmark:791318099311329280>";
        public static readonly string Crossmark = "<:crossmark:791318099022315561>";
        public static readonly string Error = "<:error:828216690452987935>";
        public static readonly string Unavailable = "<:unavailable:828216690525339658>";
        public static readonly string HighPriority = "<:high_priority:828216690164105257>";
    }
}