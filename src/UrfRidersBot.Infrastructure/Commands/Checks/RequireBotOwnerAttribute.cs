using System;
using System.Linq;
using System.Threading.Tasks;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Attributes;

namespace UrfRidersBot.Infrastructure.Commands.Checks
{
    public class RequireBotOwnerAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context, IServiceProvider provider)
        {
            var owner = context.Client.CurrentApplication.Owners.First();
            return owner.Id == context.User.Id
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("This can only be executed by the bot's owner.");
        }
    }
}