using System;
using Microsoft.Extensions.DependencyInjection;
using UrfRidersBot.Core.Common;

namespace UrfRidersBot.Persistence.Common
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IServiceProvider _provider;

        public UnitOfWorkFactory(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        public IUnitOfWork Create()
        {
            return ActivatorUtilities.CreateInstance<UnitOfWork>(_provider);
        }
    }
}