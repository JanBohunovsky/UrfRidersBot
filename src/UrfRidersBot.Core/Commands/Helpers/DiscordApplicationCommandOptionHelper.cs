using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Core.Commands.Helpers
{
    public static class DiscordApplicationCommandOptionHelper
    {
        public static DiscordApplicationCommandOption FromSlashCommandParameter(SlashCommandParameter parameter)
        {
            // Figure out type
            var type = parameter.Property.PropertyType;
            ApplicationCommandOptionType parameterType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GenericTypeArguments.First();
            }

            if (type == typeof(string))
                parameterType = ApplicationCommandOptionType.String;
            else if (type == typeof(long))
                parameterType = ApplicationCommandOptionType.Integer;
            else if (type == typeof(bool))
                parameterType = ApplicationCommandOptionType.Boolean;
            else if (type == typeof(DiscordChannel))
                parameterType = ApplicationCommandOptionType.Channel;
            else if (type == typeof(DiscordUser))
                parameterType = ApplicationCommandOptionType.User;
            else if (type == typeof(DiscordRole))
                parameterType = ApplicationCommandOptionType.Role;
            else if (type.IsEnum)
                parameterType = ApplicationCommandOptionType.String;
            else
                throw new ArgumentException($"Invalid parameter type '{type}' for parameter '{parameter.Name}'. " +
                                            $"Valid types are string, long, bool, DiscordChannel, DiscordUser, DiscordRole and enum.");
                
            // Create choices for enum
            List<DiscordApplicationCommandOptionChoice>? choices = null;
            if (type.IsEnum)
            {
                var names = Enum.GetNames(type);

                choices = names
                    .Select(name => new DiscordApplicationCommandOptionChoice(name, name))
                    .ToList();
            }

            return new DiscordApplicationCommandOption(
                parameter.Name,
                parameter.Description,
                parameterType,
                parameter.IsRequired,
                choices
            );
        }

        public static DiscordApplicationCommandOption FromSlashSubCommand(SlashCommand command)
        {
            return new DiscordApplicationCommandOption(
                command.Name,
                command.Description,
                ApplicationCommandOptionType.SubCommand,
                options: command.Parameters?.Values.Select(FromSlashCommandParameter)
            );
        }

        public static DiscordApplicationCommandOption FromSlashSubCommandGroup(
            string name, 
            string description,
            IEnumerable<SlashCommand> subCommands)
        {
            return new DiscordApplicationCommandOption(
                name,
                description,
                ApplicationCommandOptionType.SubCommandGroup,
                options: subCommands.Select(FromSlashSubCommand)
            );
        }
    }
}