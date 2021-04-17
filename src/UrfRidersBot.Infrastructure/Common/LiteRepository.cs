using LiteDB;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Infrastructure.Common
{
    internal abstract class LiteRepository<T> : IRepository
    {
        private readonly string _collectionName;
        
        protected readonly LiteDatabase Database;
        
        protected ILiteCollection<T> Collection => Database.GetCollection<T>(_collectionName);

        public LiteRepository(LiteDatabase db, string collectionName)
        {
            Database = db;
            _collectionName = collectionName;
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}