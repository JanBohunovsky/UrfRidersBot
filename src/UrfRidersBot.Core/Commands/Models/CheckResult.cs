using System.Threading.Tasks;

namespace UrfRidersBot.Core.Commands.Models
{
    public class CheckResult
    {
        public bool IsSuccessful => Reason is null;
        
        public string? Reason { get; }

        public CheckResult()
        {
        }

        public CheckResult(string reason)
        {
            Reason = reason;
        }

        public static CheckResult Successful => new CheckResult();

        public static CheckResult Unsuccessful(string reason) => new CheckResult(reason);

        public override string ToString() => IsSuccessful
            ? "Successful"
            : $"Unsuccessful: {Reason}";
        
        public static implicit operator ValueTask<CheckResult>(CheckResult result) => new(result);
    }
}