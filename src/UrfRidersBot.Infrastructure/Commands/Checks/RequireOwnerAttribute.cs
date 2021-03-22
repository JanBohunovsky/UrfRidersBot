using System.Linq;
using System.Threading.Tasks;
using Qmmands;

namespace UrfRidersBot.Infrastructure.Commands.Checks
{
    public class RequireOwnerAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext _)
        {
            var context = (UrfRidersCommandContext)_;

            var owner = context.Client.CurrentApplication.Owners.First();
            return owner.Id == context.User.Id
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("This can only be executed by the bot's owner.");
        }
    }
}