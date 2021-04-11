using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using UrfRidersBot.Core.Commands.Attributes;

namespace UrfRidersBot.Core.Commands.Built
{
    public class SlashCommand
    {
        public SlashCommandGroup? Parent { get; }
        
        /// <summary>
        /// Command's name without any group names.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Command's description
        /// </summary>
        public string Description { get; }
        
        /// <summary>
        /// The command class that implements <see cref="ICommand"/>.
        /// </summary>
        public Type Class { get; }
        
        public IReadOnlyDictionary<string, SlashCommandParameter>? Parameters { get; }
        
        public List<CheckAttribute>? Checks { get; }

        public SlashCommand(
            string name,
            string description,
            Type @class,
            ICollection<SlashCommandParameter>? parameters = null,
            ICollection<CheckAttribute>? checks = null,
            SlashCommandGroup? parent = null)
        {
            Name = name;
            Description = description;
            Class = @class;
            Parent = parent;
            
            if (parameters?.Any() == true)
            {
                Parameters = parameters.ToDictionary(p => p.Name, p => p);
            }

            if (checks?.Any() == true)
            {
                Checks = checks.ToList();
            }
        }

        public string GetFullName()
        {
            var sb = new StringBuilder();

            if (Parent is not null)
            {
                var commandGroup = Parent;
                if (commandGroup.Parent is not null)
                {
                    var groupParent = commandGroup.Parent;
                    sb.Append(groupParent.Name);
                    sb.Append(' ');
                }

                sb.Append(commandGroup.Name);
                sb.Append(' ');
            }

            sb.Append(Name);

            return sb.ToString();
        }

        public void FillParameters(ICommand instance,
            IEnumerable<DiscordInteractionDataOption> parameters,
            DiscordInteractionResolvedCollection resolved)
        {
            if (Parameters is null)
            {
                return;
            }
            
            foreach (var parameter in parameters)
            {
                Parameters[parameter.Name].SetValue(instance, parameter, resolved);
            }
        }

        public async ValueTask<CheckResult> RunChecksAsync(ICommandContext context, IServiceProvider provider)
        {
            if (Checks is null)
            {
                return CheckResult.Successful;
            }

            var results = await Task.WhenAll(Checks.Select(c => c.CheckAsync(context, provider).AsTask()));
            var failedChecks = results
                .Where(r => !r.IsSuccessful)
                .Select(r => r.Reason!)
                .ToList();

            return failedChecks.Any() 
                ? CheckResult.Unsuccessful(string.Join("\n", failedChecks))
                : CheckResult.Successful;
        }
        
    }
}