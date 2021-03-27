using System;

namespace UrfRidersBot.Core.Commands.Attributes
{
    public class ParameterAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsRequired { get; }

        public ParameterAttribute(string name, string description, bool isRequired = true)
        {
            Name = name;
            Description = description;
            IsRequired = isRequired;
        }
    }
}