using System;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Common.Commands.Entities
{
    public class ParameterDefinition
    {
        private readonly ParameterAttribute _attribute;

        public string Name => _attribute.Name;
        public string Description => _attribute.Description;
        public bool IsOptional => _attribute.IsOptional;
        
        public PropertyInfo Property { get; }
        
        public ParameterDefinition(PropertyInfo property)
        {
            Property = property;

            _attribute = property.GetCustomAttribute<ParameterAttribute>()
                         ?? throw new InvalidOperationException($"Parameter property does not have {nameof(ParameterAttribute)}: {property}");
        }
        
        public void SetValue(
            ICommand commandInstance,
            DiscordInteractionDataOption parameterOption,
            DiscordInteractionResolvedCollection data)
        {
            object value;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (parameterOption.Type)
            {
                case ApplicationCommandOptionType.Boolean:
                    value = (bool)parameterOption.Value;
                    break;
                case ApplicationCommandOptionType.Integer:
                    value = (long)parameterOption.Value;
                    break;
                case ApplicationCommandOptionType.String:
                    value = Property.PropertyType.IsEnum
                        ? Enum.Parse(Property.PropertyType, (string)parameterOption.Value)
                        : (string)parameterOption.Value;
                    break;
                case ApplicationCommandOptionType.Channel:
                    var channelId = (ulong)parameterOption.Value;
                    value = data.Channels[channelId];
                    break;
                case ApplicationCommandOptionType.Role:
                    var roleId = (ulong)parameterOption.Value;
                    value = data.Roles[roleId];
                    break;
                case ApplicationCommandOptionType.User:
                    var userId = (ulong)parameterOption.Value;
                    value = data.Members.TryGetValue(userId, out var member)
                        ? member
                        : data.Users[userId];
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported parameter type: {parameterOption.Type}");
            }

            Property.SetValue(commandInstance, value);
        }
    }
}