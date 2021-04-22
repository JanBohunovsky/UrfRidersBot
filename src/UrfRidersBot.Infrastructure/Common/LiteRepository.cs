using System.Threading.Tasks;
using LiteDB;
using LiteDB.Async;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Infrastructure.Common
{
    internal abstract class LiteRepository<T> : IRepository
    {
        private readonly string _collectionName;
        
        protected readonly ILiteDatabaseAsync Database;
        
        protected ILiteCollectionAsync<T> Collection => Database.GetCollection<T>(_collectionName);

        public LiteRepository(ILiteDatabaseAsync db, string collectionName)
        {
            Database = db;
            _collectionName = collectionName;
        }

        public void Dispose()
        {
            Database.UnderlyingDatabase.Checkpoint();
        }

        public async ValueTask DisposeAsync()
        {
            await Database.CheckpointAsync();
        }
    }
}