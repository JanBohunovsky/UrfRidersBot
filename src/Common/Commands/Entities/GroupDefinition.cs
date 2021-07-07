using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UrfRidersBot.Common.Commands.Entities
{
    public class GroupDefinition : IEquatable<GroupDefinition>
    {
        public static Dictionary<Type, GroupDefinition> Cache { get; } = new();
        
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

            if (_attribute.ParentType is null)
            {
                FullName = Name;
                return;
            }

            if (Cache.TryGetValue(_attribute.ParentType, out var parent))
            {
                Parent = parent;
            }
            else
            {
                Parent = new GroupDefinition(_attribute.ParentType);
                Cache[_attribute.ParentType] = Parent;
            }

            FullName = $"{Parent.FullName} {Name}";
        }

        private void ValidateType()
        {
            if (Type.IsInterface || Type.IsAbstract)
            {
                throw new InvalidOperationException($"Command group type must not be an interface or an abstract class: {Type}");
            }

            if (Type.GetInterfaces().All(t => t != typeof(IGroup)))
            {
                throw new InvalidOperationException($"Command group type must implement {nameof(IGroup)} interface: {Type}");
            }

            if (Type.GetCustomAttribute<GroupAttribute>() is null)
            {
                throw new InvalidOperationException($"Command group type must have {nameof(GroupAttribute)}: {Type}");
            }
        }

        public bool Equals(GroupDefinition? other)
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
            return Equals((GroupDefinition)obj);
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }
    }
}