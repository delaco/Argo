using System.Collections.Concurrent;

namespace Argo.Commands
{
    public class CommandInvokerCache
    {
        private readonly ICommandDescriptorCollectionProvider _collectionProvider;
        private volatile InnerCache _currentCache;

        public CommandInvokerCache(ICommandDescriptorCollectionProvider collectionProvider)
        {
            _collectionProvider = collectionProvider;
        }

        private InnerCache CurrentCache
        {
            get
            {
                var current = _currentCache;
                _ = _collectionProvider.CommandDescriptors;

                if (current == null)
                {
                    current = new InnerCache();
                    _currentCache = current;
                }

                return current;
            }
        }

        public object Get(CommandContext commandContext)
        {
            var cache = CurrentCache;
            var commandDescriptor = commandContext.CommandDescriptor;
            if (!cache.Entries.TryGetValue(commandDescriptor, out _))
            {
            }
            else
            {
            }

            return null;
        }

        private class InnerCache
        {
            public ConcurrentDictionary<CommandDescriptor, CommandInvokerCacheEntry> Entries { get; } =
                new ConcurrentDictionary<CommandDescriptor, CommandInvokerCacheEntry>();
        }
    }
}