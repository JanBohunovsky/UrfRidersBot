﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Infrastructure.Commands;
using UrfRidersBot.Persistence;
using DiscordConfiguration = UrfRidersBot.Core.Configuration.DiscordConfiguration;

namespace UrfRidersBot.Infrastructure
{
    internal partial class DiscordService : IHostedService
    {
        private readonly DiscordClient _client;
        private readonly DiscordConfiguration _discordConfig;
        private readonly IDbContextFactory<UrfRidersDbContext> _dbContextFactory;
        private readonly ILogger<DiscordService> _logger;
        private readonly IHostEnvironment _environment;
        private readonly IServiceProvider _provider;

        public DiscordService(
            DiscordClient client,
            DiscordConfiguration discordConfig,
            IDbContextFactory<UrfRidersDbContext> dbContextFactory,
            ILogger<DiscordService> logger,
            IHostEnvironment environment,
            IServiceProvider provider)
        {
            _client = client;
            _discordConfig = discordConfig;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _provider = provider;
            _environment = environment;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterCommands();
            RegisterInteractivity();

            _client.GetCommandsNext().CommandErrored += OnCommandErrored;
            
            // Start discord client
            await _client.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();

            _client.GetCommandsNext().CommandErrored -= OnCommandErrored;
        }
        
        private void RegisterCommands()
        {
            var commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                EnableMentionPrefix = true,
                Services = _provider,
                PrefixResolver = PrefixResolver,
            });
            
            commands.SetHelpFormatter<UrfRidersHelpFormatter>();
            commands.RegisterCommands(Assembly.GetExecutingAssembly());
        }
        
        private void RegisterInteractivity()
        {
            _client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.DeleteEmojis,
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                PaginationDeletion = PaginationDeletion.DeleteEmojis,
            });
        }
    }
}