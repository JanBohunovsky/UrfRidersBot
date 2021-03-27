using System;
using System.Collections.Generic;
using System.Linq;

namespace UrfRidersBot.Core.Commands.Entities
{
    public class SlashCommand
    {
        public SlashCommandGroup? Parent { get; }
        
        /// <summary>
        /// Command's name without any group names.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Command's description
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// The command class that implements <see cref="ICommand"/>.
        /// </summary>
        public Type Class { get; }
        
        public IReadOnlyDictionary<string, SlashCommandParameter>? Parameters { get; }
        
        // For the future:
        // public List<CommandCheckAttribute> Checks { get; set; }

        public SlashCommand(
            string name,
            string description,
            Type @class,
            ICollection<SlashCommandParameter>? parameters = null,
            SlashCommandGroup? parent = null)
        {
            Name = name;
            Description = description;
            Class = @class;
            Parent = parent;

            if (parameters is null || !parameters.Any())
            {
                return;
            }

            Parameters = parameters.ToDictionary(p => p.Name, p => p);
            
        }
    }
}