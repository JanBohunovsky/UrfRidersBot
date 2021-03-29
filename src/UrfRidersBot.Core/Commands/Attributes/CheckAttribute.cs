using System;
using System.Threading.Tasks;

namespace UrfRidersBot.Core.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class CheckAttribute : Attribute
    {
        public abstract ValueTask<CheckResult> CheckAsync(CommandContext context, IServiceProvider provider);
    }
}