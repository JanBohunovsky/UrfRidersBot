using System;

namespace UrfRidersBot.Core.Commands.Attributes
{
    public class ParameterAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        
        public ParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}