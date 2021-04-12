using System;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Attributes;

namespace UrfRidersBot.Core.Commands.Built
{
    public class SlashCommandParameter
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsRequired { get; }
        public PropertyInfo Property { get; }

        public SlashCommandParameter(ParameterAttribute attribute, PropertyInfo property)
        {
            Name = attribute.Name;
            Description = attribute.Description;
            IsRequired = attribute.IsRequired;
            Property = property;
        }

        public void SetValue(
            ICommand instance,
            DiscordInteractionDataOption parameter,
            DiscordInteractionResolvedCollection resolved)
        {
            object? result;
            switch (parameter.Type)
            {
                case ApplicationCommandOptionType.String:
                    result = Property.PropertyType.IsEnum 
                        ? Enum.Parse(Property.PropertyType, parameter.Value.ToString()!) 
                        : parameter.Value.ToString();
                    break;
                case ApplicationCommandOptionType.Integer:
                    result = (long)parameter.Value;
                    break;
                case ApplicationCommandOptionType.Boolean:
                    result = (bool)parameter.Value;
                    break;
                case ApplicationCommandOptionType.User:
                    var userId = (ulong)parameter.Value;
                    if (resolved.Members.TryGetValue(userId, out var member))
                    {
                        result = member;
                    }
                    else if (resolved.Users.TryGetValue(userId, out var user))
                    {
                        result = user;
                    }
                    else
                    {
                        throw new Exception("Could not get user.");
                    }

                    break;
                case ApplicationCommandOptionType.Channel:
                    var channelId = (ulong)parameter.Value;
                    if (resolved.Channels.TryGetValue(channelId, out var channel))
                    {
                        result = channel;
                    }
                    else
                    {
                        throw new Exception("Could not get channel.");
                    }

                    break;
                case ApplicationCommandOptionType.Role:
                    var roleId = (ulong)parameter.Value;
                    if (resolved.Roles.TryGetValue(roleId, out var role))
                    {
                        result = role;
                    }
                    else
                    {
                        throw new Exception("Could not get role.");
                    }

                    break;
                default:
                    throw new InvalidOperationException($"Invalid parameter type: '{parameter.Type}'");
            }

            Property.SetValue(instance, result);
        }
    }
}