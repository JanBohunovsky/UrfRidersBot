using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core;
using UrfRidersBot.Infrastructure.Commands;

namespace UrfRidersBot.Infrastructure.HostedServices
{
    internal partial class DiscordService
    {
        private async Task<int> PrefixResolver(DiscordMessage message)
        {
            // Check if the default prefix is valid
            if (string.IsNullOrWhiteSpace(_discordOptions.CurrentValue.Prefix))
                return -1;

            // Get custom prefix from guild
            await using var unitOfWork = _unitOfWorkFactory.Create();
            var guildSettings = await unitOfWork.GuildSettings.GetAsync(message.Channel.Guild);

            // No custom prefix (or invalid one) -> use default
            if (guildSettings?.CustomPrefix == null || string.IsNullOrWhiteSpace(guildSettings.CustomPrefix))
            {
                return message.GetStringPrefixLength(_discordOptions.CurrentValue.Prefix, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return message.GetStringPrefixLength(guildSettings.CustomPrefix, StringComparison.OrdinalIgnoreCase);
            }
        }

        private async Task OnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            _logger.LogInformation(
                e.Exception,
                "{User} tried executing '{Command}' but it errored:",
                e.Context.User,
                e.Command?.QualifiedName ?? "unknown command"
            );

            DiscordEmbedBuilder? embed;
            switch (e.Exception)
            {
                // User does not have required permission
                case ChecksFailedException:
                    embed = new DiscordEmbedBuilder
                    {
                        Color = UrfRidersColor.Red,
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = "Access denied",
                            IconUrl = UrfRidersIcon.Unavailable,
                        },
                        Description = "You do not have the permissions required to execute this command.",
                    };
                    break;
                
                // User got the command arguments wrong (wrong type, missing, extra, etc.)
                case ArgumentException argumentException:
                    var help = new UrfRidersHelpFormatter(e.Context)
                        .WithCommand(e.Command)
                        .Build();
                    
                    // Show as error with extra info from help
                    embed = new DiscordEmbedBuilder(help.Embed)
                    {
                        Title = null,
                        Color = UrfRidersColor.Yellow,
                        Author = new DiscordEmbedBuilder.EmbedAuthor
                        {
                            Name = "Error",
                            IconUrl = UrfRidersIcon.Error,
                        },
                        Description = argumentException.Message,
                    };
                    break;
                
                // User is dumb (in some cases this can be an actual system exception tho...)
                case InvalidOperationException invalidOperationException:
                    embed = EmbedHelper.CreateError(invalidOperationException.Message);
                    break;
                
                // We do not care about these exceptions
                case CommandNotFoundException:
                    embed = null;
                    break;
                
                // Unhandled case -> probably application exception
                default:
                    // Log it at higher level
                    _logger.LogError(
                        e.Exception,
                        "An exception has occured while executing '{Command}' invoked by {User}",
                        e.Command?.QualifiedName ?? "unknown command",
                        e.Context.User
                    );

                    embed = EmbedHelper.CreateCriticalError("An exception has occured.");

                    // Send the bot owner a message (if he's in this guild)
                    var owner = await e.Context.Guild.GetMemberAsync(e.Context.Client.CurrentApplication.Owners.First().Id);
                    if (owner != null && _environment.IsProduction())
                    {
                        embed.WithFooter("The bot owner has been notified.");

                        var reportEmbed = new DiscordEmbedBuilder
                        {
                            Color = UrfRidersColor.Red,
                            Author = new DiscordEmbedBuilder.EmbedAuthor
                            {
                                Name = "An exception has occured",
                                IconUrl = UrfRidersIcon.HighPriority,
                            },
                            Description = $"Here is the [message]({e.Context.Message.JumpLink}) that caused the exception, " +
                                          $"check logs for the actual exception.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter
                            {
                                Text = $"{e.Context.User.Username}#{e.Context.User.Discriminator}",
                                IconUrl = e.Context.User.GetAvatarUrl(ImageFormat.Auto),
                            },
                            Timestamp = e.Context.Message.Timestamp,
                        };

                        reportEmbed
                            .AddField("Guild", e.Context.Guild.Name, true)
                            .AddField("Channel", e.Context.Channel.Name, true)
                            .AddField("Message", Markdown.Code(e.Context.Message.Content), true);

                        await owner.SendMessageAsync(reportEmbed.Build());
                    }
                    break;
            }

            if (embed != null)
            {
                await e.Context.RespondAsync(embed.Build());
            }
        }
    }
}