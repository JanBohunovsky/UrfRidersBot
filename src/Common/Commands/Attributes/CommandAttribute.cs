using System;

namespace UrfRidersBot.Common.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; }
        
        public string Description { get; }

        public Type? ParentType { get; }
        
        /// <summary>
        /// Whether only the user who executed this command can see the response.
        /// </summary>
        public bool PrivateResponse { get; set; }

        public CommandAttribute(string name, string description, Type? parentType = null)
        {
            Name = name;
            Description = description;
            ParentType = parentType;
        }
    }
}