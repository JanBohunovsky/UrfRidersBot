using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Interfaces;

namespace UrfRidersBot.Infrastructure.HostedServices
{
    public class ReactionRolesHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ReactionRolesHostedService(DiscordClient client, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _client = client;
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReactionAdded += OnMessageReactionAdded;
            _client.MessageReactionRemoved += OnMessageReactionRemoved;
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.MessageReactionAdded -= OnMessageReactionAdded;
            _client.MessageReactionRemoved -= OnMessageReactionRemoved;
            
            return Task.CompletedTask;
        }

        private async Task OnMessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            if (e.User.IsCurrent)
                return;

            if (e.Guild == null)
                return;

            await using var unitOfWork = _unitOfWorkFactory.Create();
            var role = await unitOfWork.ReactionRoles.GetRoleAsync(e.Message, e.Emoji);

            if (role == null)
                return;

            var member = await e.Guild.GetMemberAsync(e.User.Id);
            await member.GrantRoleAsync(role);
        }

        private async Task OnMessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            if (e.User.IsCurrent)
                return;

            if (e.Guild == null)
                return;

            await using var unitOfWork = _unitOfWorkFactory.Create();
            var role = await unitOfWork.ReactionRoles.GetRoleAsync(e.Message, e.Emoji);

            if (role == null)
                return;

            var member = await e.Guild.GetMemberAsync(e.User.Id);
            await member.RevokeRoleAsync(role);
        }
    }
}