using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;

namespace UrfRidersBot.Common.Commands.Entities
{
    public class CommandDefinition : IEquatable<CommandDefinition>
    {
        private readonly CommandAttribute _attribute;

        public string Name => _attribute.Name;
        public string Description => _attribute.Description;
        public bool PrivateResponse => _attribute.PrivateResponse;
        
        public string FullName { get; }
        public Type Type { get; }
        public GroupDefinition? Parent { get; }
        public IReadOnlyDictionary<string, ParameterDefinition>? Parameters { get; }

        public CommandDefinition(Type commandType)
        {
            Type = commandType;
            ValidateType();
            
            _attribute = Type.GetCustomAttribute<CommandAttribute>()!;

            Parameters = Type.GetProperties()
                .Where(p => p.CanWrite && p.GetCustomAttribute<ParameterAttribute>() is not null)
                .Select(p => new ParameterDefinition(p))
                .ToDictionary(d => d.Name, d => d);

            if (_attribute.ParentType is null)
            {
                FullName = Name;
                return;
            }

            if (GroupDefinition.Cache.TryGetValue(_attribute.ParentType, out var parent))
            {
                Parent = parent;
            }
            else
            {
                Parent = new GroupDefinition(_attribute.ParentType);
                ValidateParent();
                GroupDefinition.Cache[_attribute.ParentType] = Parent;
            }

            FullName = $"{Parent.FullName} {Name}";
        }

        public void SetParameters(
            ICommand commandInstance, 
            IEnumerable<DiscordInteractionDataOption> parameterOptions,
            DiscordInteractionResolvedCollection data)
        {
            if (Parameters is null)
            {
                return;
            }

            foreach (var parameterOption in parameterOptions)
            {
                Parameters[parameterOption.Name].SetValue(commandInstance, parameterOption, data);
            }
        }

        private void ValidateType()
        {
            if (Type.IsInterface || Type.IsAbstract)
            {
                throw new InvalidOperationException($"Command type must not be an interface or an abstract class: {Type}");
            }

            if (Type.GetInterfaces().All(t => t != typeof(ICommand)))
            {
                throw new InvalidOperationException($"Command type must implement {nameof(ICommand)} interface: {Type}");
            }

            if (Type.GetCustomAttribute<CommandAttribute>() is null)
            {
                throw new InvalidOperationException($"Command type must have {nameof(CommandAttribute)}: {Type}");
            }
        }

        private void ValidateParent()
        {
            if (Parent?.Parent?.Parent is not null)
            {
                throw new InvalidOperationException($"Command can only be nested up to 2 levels (Group > Group > Command): {Type}");
            }
        }

        public DiscordApplicationCommandOption ToDiscordSubCommand()
        {
            return new(
                Name,
                Description,
                ApplicationCommandOptionType.SubCommand,
                options: Parameters?.Values.Select(p => p.ToDiscord())
            );
        }

        public bool Equals(CommandDefinition? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return FullName == other.FullName;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((CommandDefinition)obj);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
    }
}