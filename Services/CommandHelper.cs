using Discord.Commands;
using LiteDB;
using System.Text;

namespace UrfRiders.Services
{
    public class CommandHelper
    {
        private readonly LiteDatabase _database;

        public CommandHelper(LiteDatabase database)
        {
            _database = database;
        }

        public string Usage(CommandInfo command, ulong? guildId = null)
        {
            var prefix = guildId.HasValue ? new ServerSettings(guildId.Value, _database).Prefix : "";

            var commandBuilder = new StringBuilder();
            // TODO: don't forget for group command
            // e.g. "rr add <type> <emoji> <role>" is currently displayed without "rr"
            commandBuilder.Append($"{prefix}{command.Name}");

            foreach (var parameter in command.Parameters)
            {
                if (parameter.IsOptional)
                    commandBuilder.Append($" [{parameter.Name} = {parameter.DefaultValue}]");
                else if (parameter.IsMultiple)
                    commandBuilder.Append($" |{parameter.Name}|");
                //else if (parameter.IsRemainder)
                //    commandBuilder.Append($" ...{parameter.Name}");
                else
                    commandBuilder.Append($" <{parameter.Name}>");
            }

            return commandBuilder.ToString();
        }
    }
}