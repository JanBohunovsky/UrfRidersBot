namespace UrfRidersBot.Core.Commands.Entities
{
    public class SlashCommandGroup
    {
        public SlashCommandGroup? Parent { get; }
        
        public string Name { get; }
        
        public string Description { get; }
        
        public SlashCommandGroup(string name, string description, SlashCommandGroup? parent = null)
        {
            Name = name;
            Description = description;
            Parent = parent;
        }
    }
}