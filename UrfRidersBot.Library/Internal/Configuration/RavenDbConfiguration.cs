using Microsoft.Extensions.Configuration;

namespace UrfRidersBot.Library.Internal.Configuration
{
    internal class RavenDbConfiguration
    {
        public string Url { get; set; } = "http://localhost:8080";
        public string Database { get; set; } = "Default";
        public string? CertificatePath { get; set; }

        public RavenDbConfiguration(IConfiguration configuration)
        {
            configuration.Bind("RavenDB", this);
        }
    }
}