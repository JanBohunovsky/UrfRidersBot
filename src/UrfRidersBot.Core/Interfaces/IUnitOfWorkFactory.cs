namespace UrfRidersBot.Core.Interfaces
{
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// The caller is responsible for disposing the unit of work.
        /// </summary>
        IUnitOfWork Create();
    }
}