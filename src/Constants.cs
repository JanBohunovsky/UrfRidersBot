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
        // TODO: Move these to Github repo: UrfRidersBot/images
        public static readonly string Checkmark = "https://cdn.discordapp.com/attachments/717788228899307551/791314528280641546/icons8-checkmark-96.png";
        public static readonly string Ok = "https://cdn.discordapp.com/attachments/717788228899307551/791314535780450344/icons8-ok-96.png";
        public static readonly string Error = "https://cdn.discordapp.com/attachments/717788228899307551/791314543917006928/icons8-error-96.png";
        public static readonly string HighPriority = "https://cdn.discordapp.com/attachments/717788228899307551/791314550561570876/icons8-high-priority-96.png";
        public static readonly string Checked = "https://cdn.discordapp.com/attachments/717788228899307551/791314567954563122/icons8-checked-96.png";
        public static readonly string Cancel = "https://cdn.discordapp.com/attachments/717788228899307551/791314575093268520/icons8-cancel-96.png";
        public static readonly string Unavailable = "https://cdn.discordapp.com/attachments/717788228899307551/791314583591059466/icons8-unavailable-96.png";
        public static readonly string Delete = "https://cdn.discordapp.com/attachments/717788228899307551/791314591657099274/icons8-delete-96.png";
    }
}