using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using UrfRidersBot.Core.Commands.Built;

namespace UrfRidersBot.Infrastructure.Commands.Models
{
    public class DiscordCommandOption
    {
        [JsonProperty("type")]
        public ApplicationCommandOptionType Type { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; } = "";

        [JsonProperty("description")]
        public string Description { get; init; } = "";

        [JsonProperty("required", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Required { get; init; }

        [JsonProperty("choices", NullValueHandling = NullValueHandling.Ignore)]
        public List<DiscordApplicationCommandOptionChoice>? Choices { get; init; }

        [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
        public List<DiscordCommandOption>? Options { get; init; }

        public static DiscordCommandOption FromParameter(SlashCommandParameter parameter)
        {
            // Figure out type
            var type = parameter.Property.PropertyType;
            ApplicationCommandOptionType parameterType;
            
            if (type == typeof(string))
                parameterType = ApplicationCommandOptionType.String;
            else if (type == typeof(int))
                parameterType = ApplicationCommandOptionType.Integer;
            else if (type == typeof(uint))
                parameterType = ApplicationCommandOptionType.Integer;
            else if (type == typeof(long))
                parameterType = ApplicationCommandOptionType.Integer;
            else if (type == typeof(ulong))
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
                parameterType = ApplicationCommandOptionType.Integer;
            else
                throw new ArgumentException(
                    "Cannot convert type. Parameter type must be string, int, uint, long, ulong, bool, DiscordChannel, DiscordUser, DiscordRole or an enum.");
                
            // Create choices for enum
            List<DiscordApplicationCommandOptionChoice>? choices = null;
            if (type.IsEnum)
            {
                choices = new List<DiscordApplicationCommandOptionChoice>();
                var names = Enum.GetNames(type);
                var values = Enum.GetValues(type);
                
                for (int i = 0; i < names.Length; i++)
                {
                    var name = names[i];
                    var value = (int)values.GetValue(i)!;
                    choices.Add(new DiscordApplicationCommandOptionChoice(name, value));
                }
            }
            
            return new DiscordCommandOption
            {
                Name = parameter.Name,
                Description = parameter.Description,
                Required = parameter.IsRequired,
                Type = parameterType,
                Choices = choices
            };
        }

        public static DiscordCommandOption FromSubCommand(SlashCommand command)
        {
            return new DiscordCommandOption
            {
                Name = command.Name,
                Description = command.Description,
                Type = ApplicationCommandOptionType.SubCommand,
                Options = command.Parameters?.Values.Select(FromParameter).ToList()
            };
        }

        public static DiscordCommandOption FromSubCommandGroup(string name, string description, List<SlashCommand> subCommands)
        {
            return new DiscordCommandOption
            {
                Name = name,
                Description = description,
                Type = ApplicationCommandOptionType.SubCommandGroup,
                Options = subCommands.Select(FromSubCommand).ToList()
            };
        }
    }
}