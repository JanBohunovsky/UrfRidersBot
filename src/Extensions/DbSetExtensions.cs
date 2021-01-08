using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UrfRidersBot
{
    public static class DbSetExtensions
    {
        public static async ValueTask<TEntity> FindOrCreateAsync<TEntity, TKey>(
            this DbSet<TEntity> dbSet,
            TKey key,
            Func<TKey, TEntity> entityFactory)
            where TEntity : class
        {
            var entity = await dbSet.FindAsync(key);
            if (entity == null)
            {
                entity = entityFactory(key);
                await dbSet.AddAsync(entity);
            }

            return entity;
        }

        public static TEntity FindOrCreate<TEntity, TKey>(
            this DbSet<TEntity> dbSet,
            TKey key,
            Func<TKey, TEntity> entityFactory)
            where TEntity : class
        {
            var entity = dbSet.Find(key);
            if (entity == null)
            {
                entity = entityFactory(key);
                dbSet.Add(entity);
            }

            return entity;
        }
    }
}