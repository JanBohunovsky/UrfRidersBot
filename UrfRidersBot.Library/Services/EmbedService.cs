﻿using Discord;
using Discord.WebSocket;

namespace UrfRidersBot.Library
{
    internal class EmbedService : IEmbedService
    {
        private readonly BotConfiguration _botConfig;
        private readonly DiscordSocketClient _discord;

        public EmbedService(BotConfiguration botConfig, DiscordSocketClient discord)
        {
            _botConfig = botConfig;
            _discord = discord;
        }

        public EmbedBuilder CreateBasic(string? description = null, string? title = null)
        {
            var builder = new EmbedBuilder()
                .WithColor(_botConfig.EmbedColor);

            if (description != null)
                builder.WithDescription(description);
            if (title != null)
                builder.WithTitle(title);

            return builder;
        }

        public EmbedBuilder CreateBotInfo(string? nameSuffix = null)
        {
            var name = nameSuffix == null ? _botConfig.Name : $"{_botConfig.Name} {nameSuffix}";

            var builder = new EmbedBuilder()
                .WithColor(_botConfig.EmbedColor)
                .WithAuthor(name, _discord.CurrentUser.GetAvatarUrl());

            return builder;
        }

        public EmbedBuilder CreateSuccess(string? description = null, string title = "Success")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithAuthor(title, "https://cdn.discordapp.com/attachments/717788228899307551/791314528280641546/icons8-checkmark-96.png");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }

        public EmbedBuilder CreateError(string? description = null, string? title = "Error")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Gold)
                .WithAuthor(title, "https://cdn.discordapp.com/attachments/717788228899307551/791314543917006928/icons8-error-96.png");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }

        public EmbedBuilder CreateCriticalError(string? description = null, string? title = "Critical Error")
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(title, "https://cdn.discordapp.com/attachments/717788228899307551/791314550561570876/icons8-high-priority-96.png");
            
            if (description != null)
                builder.WithDescription(description);

            return builder;
        }
    }
}