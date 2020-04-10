using Discord.Commands;
using LiteDB;
using UrfRiders.Services;

namespace UrfRiders.Modules
{
    public class BaseModule : ModuleBase<SocketCommandContext>
    {
        public LiteDatabase Database { get; set; }
        public ServerSettings Settings => _settings ??= new ServerSettings(Context.Guild.Id, Database);

        private ServerSettings _settings;
    }
}