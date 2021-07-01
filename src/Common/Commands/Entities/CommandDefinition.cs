using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UrfRidersBot.Common.Commands.Entities
{
    public class CommandDefinition
    {
        private readonly CommandAttribute _attribute;

        public string Name => _attribute.Name;
        public string Description => _attribute.Description;
        public bool EphemeralResponse => _attribute.PrivateResponse;
        
        public string FullName { get; }
        public Type Type { get; }
        public CommandGroupDefinition? Parent { get; }
        public IReadOnlyDictionary<string, CommandParameterDefinition>? Parameters { get; }

        public CommandDefinition(Type commandType)
        {
            Type = commandType;
            ValidateType();
            
            _attribute = Type.GetCustomAttribute<CommandAttribute>()!;

            Parameters = Type.GetProperties()
                .Where(p => p.CanWrite && p.GetCustomAttribute<ParameterAttribute>() is not null)
                .Select(p => new CommandParameterDefinition(p))
                .ToDictionary(d => d.Name, d => d);

            if (_attribute.ParentType is not null)
            {
                Parent = new CommandGroupDefinition(_attribute.ParentType);
                ValidateParent();
            }

            FullName = $"{Parent?.FullName} {Name}".Trim();
        }

        // public void SetParameters(ICommand commandInstance, discordOptions, discordResolvedCollection)

        private void ValidateType()
        {
            if (Type.IsInterface || Type.IsAbstract)
            {
                throw new InvalidOperationException($"Command type must not be an interface or an abstract class: {Type}");
            }

            if (Type.GetInterfaces().Any(t => t == typeof(ICommand)))
            {
                throw new InvalidOperationException($"Command type must implement ICommand interface: {Type}");
            }

            if (Type.GetCustomAttribute<CommandAttribute>() is null)
            {
                throw new InvalidOperationException($"Command type must have CommandAttribute: {Type}");
            }
        }

        private void ValidateParent()
        {
            if (Parent?.Parent?.Parent is not null)
            {
                throw new InvalidOperationException($"Command can only be nested up to 2 levels (Group > Group > Command): {Type}");
            }
        }
    }
}