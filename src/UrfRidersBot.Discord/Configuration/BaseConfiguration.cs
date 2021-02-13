using Microsoft.Extensions.Configuration;

namespace UrfRidersBot.Discord.Configuration
{
    public abstract class BaseConfiguration
    {
        /// <summary>
        /// Initializes the configuration without binding.
        /// </summary>
        protected BaseConfiguration()
        {
        }
        
        /// <summary>
        /// Initializes the instance by binding settable properties.
        /// </summary>
        protected BaseConfiguration(IConfiguration configuration, string key)
        {
            configuration.Bind(key, this);
        }
    }
}