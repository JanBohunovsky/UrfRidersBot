namespace UrfRidersBot.Library.Internal.Configuration
{
    public class RavenDbConfiguration
    {
        public string Url { get; set; } = "http://localhost:8080";
        public string Database { get; set; } = "Default";
        public string? CertificatePath { get; set; }
    }
}