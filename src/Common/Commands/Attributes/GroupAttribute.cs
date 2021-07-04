using System;

namespace UrfRidersBot.Common.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GroupAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public Type? ParentType { get; }

        public GroupAttribute(string name, string description, Type? parentType = null)
        {
            Name = name;
            Description = description;
            ParentType = parentType;
        }
    }
}