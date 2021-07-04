using System;
using System.Linq;
using System.Reflection;

namespace UrfRidersBot.Common.Commands.Entities
{
    public class GroupDefinition
    {
        private readonly GroupAttribute _attribute;

        public string Name => _attribute.Name;
        public string Description => _attribute.Description;
        
        public string FullName { get; }
        public Type Type { get; }
        public GroupDefinition? Parent { get; }
        
        public GroupDefinition(Type commandGroupType)
        {
            Type = commandGroupType;
            ValidateType();

            _attribute = commandGroupType.GetCustomAttribute<GroupAttribute>()!;

            if (_attribute.ParentType is not null)
            {
                Parent = new GroupDefinition(_attribute.ParentType);
            }

            FullName = $"{Parent?.FullName} {Name}".Trim();
        }

        private void ValidateType()
        {
            if (Type.IsInterface || Type.IsAbstract)
            {
                throw new InvalidOperationException($"Command group type must not be an interface or an abstract class: {Type}");
            }

            if (Type.GetInterfaces().Any(t => t == typeof(IGroup)))
            {
                throw new InvalidOperationException($"Command group type must implement {nameof(IGroup)} interface: {Type}");
            }

            if (Type.GetCustomAttribute<GroupAttribute>() is null)
            {
                throw new InvalidOperationException($"Command group type must have {nameof(GroupAttribute)}: {Type}");
            }
        }
    }
}