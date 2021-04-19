using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using UrfRidersBot.Core.Common;
using UrfRidersBot.Core.ReactionRoles;

namespace UrfRidersBot.Infrastructure.ReactionRoles
{
    internal class ReactionRolesHostedService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly IRepositoryFactory<IReactionRoleRepository> _factory;

        public ReactionRolesHostedService(DiscordClient client,IRepositoryFactory<IReactionRoleRepository> factory)
        {
            _client = client;
            _factory = factory;
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

            using var repository = _factory.Create();
            var role = await repository.GetRoleAsync(e.Message, e.Emoji);

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

            using var repository = _factory.Create();
            var role = await repository.GetRoleAsync(e.Message, e.Emoji);

            if (role == null)
                return;

            var member = await e.Guild.GetMemberAsync(e.User.Id);
            await member.RevokeRoleAsync(role);
        }
    }
}