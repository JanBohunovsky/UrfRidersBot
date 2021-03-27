namespace UrfRidersBot.Core.Commands
{
    public interface ICommandGroup
    {
        string Name { get; }
        string Description { get; }
    }
    
    public interface ICommandGroup<TParent> : ICommandGroup where TParent : ICommandGroup
    {
    }
}