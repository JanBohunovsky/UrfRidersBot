using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using UrfRidersBot.Library.Internal.Configuration;

namespace UrfRidersBot.Library.Internal.DataAccess
{
    internal static class RavenDb
    {
        public static IDocumentStore Configure(IServiceProvider provider)
        {
            var config = provider.GetRequiredService<RavenDbConfiguration>();

            var store = new DocumentStore
            {
                Urls = new[] { config.Url },
                Database = config.Database,
                Conventions =
                {
                    FindCollectionName = FindCollectionName,
                }
            };

            if (config.CertificatePath != null)
            {
                store.Certificate = new X509Certificate2(config.CertificatePath);
            }

            store.Initialize();
            return store;
        }

        private static string FindCollectionName(Type type)
        {
            // TODO: Add custom collection names if needed
            return DocumentConventions.DefaultGetCollectionName(type);
        }
    }
}