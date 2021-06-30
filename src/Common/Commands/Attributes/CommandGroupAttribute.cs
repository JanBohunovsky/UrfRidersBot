using System;

namespace UrfRidersBot.Common.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CommandGroupAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public Type? ParentType { get; }

        public CommandGroupAttribute(string name, string description, Type? parentType = null)
        {
            Name = name;
            Description = description;
            ParentType = parentType;
        }
    }
}