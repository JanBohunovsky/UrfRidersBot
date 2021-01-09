using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UrfRidersBot
{
    public partial class DiscordService
    {
        private async Task<int> PrefixResolver(DiscordMessage message)
        {
            // Check if the default prefix is valid
            if (string.IsNullOrWhiteSpace(_botConfig.Prefix))
                return -1;

            // Get custom prefix from guild
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var guildSettings = await dbContext.GuildSettings.FindAsync(message.Channel.GuildId);

            // No custom prefix (or invalid one) -> use default
            if (guildSettings?.CustomPrefix == null || string.IsNullOrWhiteSpace(guildSettings.CustomPrefix))
            {
                return message.GetStringPrefixLength(_botConfig.Prefix, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return message.GetStringPrefixLength(guildSettings.CustomPrefix, StringComparison.OrdinalIgnoreCase);
            }
        }

        private async Task OnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
        {
            _client.GuildDownloadCompleted -= OnGuildDownloadCompleted;
            
            // This is the "true" Ready event, this will contain all the information in guilds
            foreach (var service in _provider.GetServices<IOnReadyService>())
            {
                await service.OnReady(sender);
            }

            // Add interactivity extensions (now we have all the emotes loaded)
            ConfigureInteractivity();
        }

        private async Task OnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            _logger.LogInformation(
                e.Exception,
                "{user} tried executing '{command}' but it errored:",
                e.Context.User,
                e.Command?.QualifiedName ?? "unknown command"
            );
            
            // Application exception has occured
            if (e.Exception.InnerException != null)
            {
                // Log it at higher level
                _logger.LogError(
                    e.Exception.InnerException,
                    "An exception has occured while executing '{command}' invoked by {user}.",
                    e.Command?.QualifiedName ?? "unknown command",
                    e.Context.User
                );
                
                // Send user information message that something went wrong
                var embed = _embedService.CreateCriticalError("An exception has occured.");
                await e.Context.RespondAsync(embed.Build());

                // Don't do anything else
                return;
            }
            
            // Check permissions and let the user know if they dont have them
            if (e.Exception is ChecksFailedException)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = UrfRidersColor.Red,
                    Author = new DiscordEmbedBuilder.EmbedAuthor
                    {
                        Name = "Access denied",
                        IconUrl = UrfRidersIcon.Unavailable,
                    },
                    Description = "You do not have the permissions required to execute this command.",
                };
                await e.Context.RespondAsync(embed.Build());
            }
        }
    }
}