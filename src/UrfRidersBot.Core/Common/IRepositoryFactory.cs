namespace UrfRidersBot.Core.Common
{
    public interface IRepositoryFactory<out T> where T : IRepository
    {
        T Create();
    }
}