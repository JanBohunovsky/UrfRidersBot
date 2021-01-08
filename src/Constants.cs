using DSharpPlus.Entities;

namespace UrfRidersBot
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
    }

    public static class UrfRidersIcon
    {
        // TODO: Remember to update the URLs when merging into master
        public static readonly string Checkmark = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/checkmark.png";
        public static readonly string Ok = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/ok.png";
        public static readonly string Error = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/error.png";
        public static readonly string HighPriority = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/high_priority.png";
        public static readonly string Checked = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/checked.png";
        public static readonly string Cancel = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/cancel.png";
        public static readonly string Unavailable = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/unavailable.png";
        public static readonly string Delete = "https://raw.githubusercontent.com/JanBohunovsky/UrfRidersBot/v2.0/images/delete.png";
    }
}