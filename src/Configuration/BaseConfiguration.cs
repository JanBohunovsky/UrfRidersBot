using Microsoft.Extensions.Configuration;

namespace UrfRidersBot
{
    public abstract class BaseConfiguration
    {
        protected BaseConfiguration(IConfiguration configuration, string key)
        {
            configuration.Bind(key, this);
        }
    }
}