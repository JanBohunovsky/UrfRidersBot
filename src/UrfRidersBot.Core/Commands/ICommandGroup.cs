namespace UrfRidersBot.Core.Commands
{
    public interface ICommandGroup
    {
        string Name { get; }
        string Description { get; }
    }
    
    public interface ICommandGroup<TGroup> : ICommandGroup where TGroup : ICommandGroup
    {
    }
}