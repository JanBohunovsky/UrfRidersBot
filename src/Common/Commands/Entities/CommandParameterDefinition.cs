using System;
using System.Reflection;

namespace UrfRidersBot.Common.Commands.Entities
{
    public class CommandParameterDefinition
    {
        private readonly ParameterAttribute _attribute;

        public string Name => _attribute.Name;
        public string Description => _attribute.Description;
        public bool IsOptional => _attribute.IsOptional;
        
        public PropertyInfo Property { get; }
        
        public CommandParameterDefinition(PropertyInfo property)
        {
            Property = property;

            _attribute = property.GetCustomAttribute<ParameterAttribute>()
                         ?? throw new InvalidOperationException($"Parameter property does not have ParameterAttribute: {property}");
        }
        
        // public void SetValue(ICommand commandInstance, discordOption, discordResolvedCollection)
    }
}