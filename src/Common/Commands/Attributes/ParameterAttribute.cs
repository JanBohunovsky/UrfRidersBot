using System;

namespace UrfRidersBot.Common.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ParameterAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsOptional { get; }
        

        public ParameterAttribute(string name, string description, bool isOptional = false)
        {
            Name = name;
            Description = description;
            IsOptional = isOptional;
        }
    }
}