using System.Threading.Tasks;
using UrfRidersBot.Common.Commands;

namespace UrfRidersBot.Admin.Commands
{
    [Command("echo", "Responds back with the input message.", typeof(TestGroup))]
    public class EchoCommand : ICommand
    {
        [Parameter("message", "Message to echo.")]
        public string Message { get; set; } = null!;

        public async Task HandleAsync(CommandContext context)
        {
            await context.RespondAsync(Message);
        }
    }
}