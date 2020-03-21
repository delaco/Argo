using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Argo.Commands
{
    public class CommandDescriptorContainer : ICommandDescriptorContainer
    {
        private ICommandDescriptorCollectionProvider _commandDescriptorCollectionProvider;
        private InnerCache _cache;

        public CommandDescriptorContainer(ICommandDescriptorCollectionProvider commandDescriptorCollectionProvider)
        {
            _commandDescriptorCollectionProvider = commandDescriptorCollectionProvider;
        }

        private InnerCache Current
        {
            get
            {
                var actions = _commandDescriptorCollectionProvider.CommandDescriptors;
                var cache = Volatile.Read(ref _cache);

                if (cache != null)
                {
                    return cache;
                }

                cache = new InnerCache(actions);
                Volatile.Write(ref _cache, cache);

                return cache;
            }
        }

        public CommandDescriptor Get(RequestContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var cache = Current;

            if (cache.Entries.TryGetValue(context.Message.Command, out CommandDescriptor descriptor))
            {
                return descriptor;
            }

            return default;
        }

        private class InnerCache
        {
            public ConcurrentDictionary<object, CommandDescriptor> Entries { get; } =
                new ConcurrentDictionary<object, CommandDescriptor>();

            public InnerCache(CommandDescriptorCollection commands)
            {
                foreach (var descriptor in commands.Items)
                {
                    Entries.TryAdd(descriptor.Key, descriptor);
                }
            }
        }
    }
}