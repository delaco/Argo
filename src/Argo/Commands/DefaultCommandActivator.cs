using System;

namespace Argo.Commands
{
    /// <summary>
    /// <see cref="ICommandActivator"/> that uses type activation to create commands.
    /// </summary>
    public class DefaultCommandActivator : ICommandActivator
    {
        private readonly ITypeActivatorCache _typeActivatorCache;

        public DefaultCommandActivator(ITypeActivatorCache typeActivatorCache)
        {
            _typeActivatorCache = typeActivatorCache ?? throw new ArgumentNullException(nameof(typeActivatorCache));
        }

        public virtual object Create(CommandContext commandContext)
        {
            if (commandContext == null)
            {
                throw new ArgumentNullException(nameof(commandContext));
            }

            if (commandContext.CommandDescriptor == null)
            {
                throw new ArgumentException($"The '{nameof(CommandContext.CommandDescriptor)}' property of '" +
                    $"{nameof(CommandContext)}' must not be null.");
            }

            var commandTypeInfo = commandContext.CommandDescriptor.CommandTypeInfo;

            if (commandTypeInfo == null)
            {
                throw new ArgumentException($"The '{nameof(commandContext.CommandDescriptor.CommandTypeInfo)}' property of '" +
                    $"{nameof(CommandContext.CommandDescriptor)}' must not be null.");
            }

            var serviceProvider = commandContext.ServiceProvider;
            return _typeActivatorCache.CreateInstance<object>(serviceProvider, commandTypeInfo.AsType());
        }

        public void Release(CommandContext commandContext, object command)
        {
            if (commandContext == null)
            {
                throw new ArgumentNullException(nameof(commandContext));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            IDisposable disposable = command as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}