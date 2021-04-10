﻿using System.Threading.Tasks;

namespace UrfRidersBot.Core.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }

        Task HandleAsync(CommandContext context);
    }
    
    public interface ICommand<TParent> : ICommand where TParent : ICommandGroup
    {
    }
}