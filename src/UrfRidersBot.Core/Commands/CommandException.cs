using System;

namespace UrfRidersBot.Core.Commands
{
    public class CommandException : Exception
    {
        public CommandException(string message) : base(message)
        {
            
        }
    }
}