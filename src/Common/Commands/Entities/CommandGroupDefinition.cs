using System;
using System.Linq;
using System.Reflection;

namespace UrfRidersBot.Common.Commands.Entities
{
    public class CommandGroupDefinition
    {
        private readonly CommandGroupAttribute _attribute;

        public string Name => _attribute.Name;
        public string Description => _attribute.Description;
        
        public string FullName { get; }
        public Type Type { get; }
        public CommandGroupDefinition? Parent { get; }
        
        public CommandGroupDefinition(Type commandGroupType)
        {
            Type = commandGroupType;
            ValidateType();

            _attribute = commandGroupType.GetCustomAttribute<CommandGroupAttribute>()!;

            if (_attribute.ParentType is not null)
            {
                Parent = new CommandGroupDefinition(_attribute.ParentType);
            }

            FullName = $"{Parent?.FullName} {Name}".Trim();
        }

        private void ValidateType()
        {
            if (Type.IsInterface || Type.IsAbstract)
            {
                throw new InvalidOperationException($"Command group type must not be an interface or an abstract class: {Type}");
            }

            if (Type.GetInterfaces().Any(t => t == typeof(ICommandGroup)))
            {
                throw new InvalidOperationException($"Command group type must implement ICommandGroup interface: {Type}");
            }

            if (Type.GetCustomAttribute<CommandGroupAttribute>() is null)
            {
                throw new InvalidOperationException($"Command group type must have CommandGroupAttribute: {Type}");
            }
        }
    }
}