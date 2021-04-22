using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrfRidersBot.Core.Commands;
using UrfRidersBot.Core.Commands.Built;
using UrfRidersBot.Core.Commands.Services;

namespace UrfRidersBot.Infrastructure.Commands.Services
{
    internal class CommandManager : ICommandManager
    {
        private readonly ILogger<CommandManager> _logger;
        private readonly IServiceProvider _provider;

        private readonly HashSet<string> _commands;
        private readonly Dictionary<string, SlashCommandGroup> _groups;
        private readonly Dictionary<string, Dictionary<string, SlashCommandGroup>> _subGroups;

        public CommandManager(ILogger<CommandManager> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;

            _commands = new HashSet<string>();
            _groups = new Dictionary<string, SlashCommandGroup>();
            _subGroups = new Dictionary<string, Dictionary<string, SlashCommandGroup>>();
        }

        public IEnumerable<SlashCommandDefinition> BuildCommands()
        {
            _commands.Clear();
            _groups.Clear();
            _subGroups.Clear();

            foreach (var commandClass in GetCommandClasses())
            {
                yield return CreateCommand(commandClass);
            }
        }

        private IEnumerable<Type> GetCommandClasses()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface);
        }

        private SlashCommandDefinition CreateCommand(Type commandClass)
        {
            if (ActivatorUtilities.CreateInstance(_provider, commandClass) is not ICommand command)
            {
                throw new Exception("Could not create ICommand instance");
            }
            
            var parameters = CreateCommandParameters(commandClass).ToList();
            var checks = GetChecks(commandClass).ToList();
            var parent = GetCommandParent(commandClass);
            var definition = new SlashCommandDefinition(command, parameters, checks, parent);

            if (parent is not null) 
                return definition;
            
            // This is a root command, so we need to check for name collisions and add it to the hashset.
            if (_commands.Contains(command.Name) || _groups.ContainsKey(command.Name))
            {
                throw new Exception($"A command or a command group already exists with this name: '{command.Name}'");
            }
            _commands.Add(definition.Name);

            return definition;
        }

        private IEnumerable<SlashCommandParameter> CreateCommandParameters(Type commandClass)
        {
            var parameterProperties = commandClass.GetProperties()
                .Where(p => p.CanWrite && p.GetCustomAttribute<ParameterAttribute>() is not null);

            foreach (var propertyInfo in parameterProperties)
            {
                var attribute = propertyInfo.GetCustomAttribute<ParameterAttribute>();
                if (attribute is null)
                {
                    throw new Exception("What?");
                }

                yield return new SlashCommandParameter(attribute, propertyInfo);
            }
        }

        private IEnumerable<CheckAttribute> GetChecks(Type commandOrGroupClass)
        {
            var checks = new List<CheckAttribute>();
            
            var parentClass = commandOrGroupClass.GetTypeInfo()
                .ImplementedInterfaces
                .Where(t => t.IsGenericType
                            && (t.GetGenericTypeDefinition() == typeof(ICommand<>)
                                || t.GetGenericTypeDefinition() == typeof(ICommandGroup<>)))
                .Select(t => t.GenericTypeArguments.First())
                .FirstOrDefault();

            if (parentClass is not null)
            {
                checks.AddRange(GetChecks(parentClass));
            }
            
            checks.AddRange(commandOrGroupClass.GetCustomAttributes<CheckAttribute>());

            return checks;
        }

        private SlashCommandGroup? GetCommandParent(Type commandClass)
        {
            var groupClass = commandClass.GetTypeInfo()
                .ImplementedInterfaces
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ICommand<>))
                .Select(t => t.GenericTypeArguments.First())
                .FirstOrDefault();

            if (groupClass is null)
            {
                return null;
            }

            if (ActivatorUtilities.CreateInstance(_provider, groupClass) is not ICommandGroup instance)
            {
                throw new Exception("Could not create ICommandGroup instance");
            }

            // Figure out if this group is a sub-group or not, and return correct dictionary based on that
            var parent = GetGroupParent(groupClass);
            var groups = parent is null
                ? _groups
                : _subGroups[parent.Name];

            if (groups.ContainsKey(instance.Name))
            {
                return groups[instance.Name];
            }

            if (parent is null && _commands.Contains(instance.Name))
            {
                throw new Exception($"A command with this name already exists: '{instance.Name}'");
            }

            groups[instance.Name] = new SlashCommandGroup(instance.Name, instance.Description, parent);
            return groups[instance.Name];
        }

        private SlashCommandGroup? GetGroupParent(Type groupClass)
        {
            var parentGroupClass = groupClass.GetTypeInfo()
                .ImplementedInterfaces
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ICommandGroup<>))
                .Select(t => t.GenericTypeArguments.First())
                .FirstOrDefault();

            if (parentGroupClass is null)
            {
                return null;
            }

            if (ActivatorUtilities.CreateInstance(_provider, parentGroupClass) is not ICommandGroup instance)
            {
                throw new Exception("Could not create ICommandGroup instance");
            }

            if (_groups.ContainsKey(instance.Name))
            {
                return _groups[instance.Name];
            }

            if (_commands.Contains(instance.Name))
            {
                throw new Exception($"A command with this name already exists: '{instance.Name}'");
            }

            _groups[instance.Name] = new SlashCommandGroup(instance.Name, instance.Description);
            _subGroups[instance.Name] = new Dictionary<string, SlashCommandGroup>();

            return _groups[instance.Name];
        }
    }
}