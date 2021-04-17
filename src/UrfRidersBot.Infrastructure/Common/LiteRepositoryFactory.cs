using System;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Infrastructure.Common
{
    internal class LiteRepositoryFactory<T> : IRepositoryFactory<T> where T : IRepository
    {
        private readonly IServiceProvider _provider;

        public LiteRepositoryFactory(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        public T Create()
        {
            return _provider.GetRequiredService<T>();
        }
    }
}