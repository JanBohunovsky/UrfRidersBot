using System.Reflection;
using UrfRidersBot.Core.Commands.Attributes;

namespace UrfRidersBot.Core.Commands.Entities
{
    public class SlashCommandParameter
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsRequired { get; }
        public PropertyInfo Property { get; }

        public SlashCommandParameter(ParameterAttribute attribute, PropertyInfo property)
        {
            Name = attribute.Name;
            Description = attribute.Description;
            IsRequired = attribute.IsRequired;
            Property = property;
        }
    }
}