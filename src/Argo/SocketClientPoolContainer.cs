using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;

namespace Argo
{
    public interface ISocketClientPoolContainer
    {
        SocketClient Get(string remoteName);

        void Return(SocketClient client);
    }

    public class SocketClientPoolContainer : ISocketClientPoolContainer
    {
        private readonly ConcurrentDictionary<string, ObjectPool<SocketClient>> _objectPoolDict
            = new ConcurrentDictionary<string, ObjectPool<SocketClient>>();

        private RemoteOptions _options;
        private ISocketClientProvider _socketClientProvider;
        private ObjectPoolProvider _objectPoolProvider;

        public SocketClientPoolContainer(IOptions<RemoteOptions> options,
            ISocketClientProvider socketClientProvider,
            ObjectPoolProvider objectPoolProvider)
        {
            this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _objectPoolProvider = objectPoolProvider ?? throw new ArgumentNullException(nameof(objectPoolProvider));
            _socketClientProvider = socketClientProvider ?? throw new ArgumentNullException(nameof(socketClientProvider));
        }

        public SocketClient Get(string remoteName)
        {
            if (!_objectPoolDict.TryGetValue(remoteName, out ObjectPool<SocketClient> objectPool))
            {
                var option = _options.Remotes.Find(v => v.Name == remoteName);
                if (option == null)
                {
                    throw new IndexOutOfRangeException(nameof(remoteName));
                }

                if (_objectPoolProvider is DefaultObjectPoolProvider defaultObjectPoolProvider)
                {
                    int poolSize = option.PoolSize;
                    defaultObjectPoolProvider.MaximumRetained = poolSize;
                }

                objectPool = _objectPoolProvider.Create(new SocketClientPooledObjectPolicy(option, _socketClientProvider));
                _objectPoolDict.TryAdd(remoteName, objectPool);
            }

            return objectPool.Get();
        }

        public void Return(SocketClient client)
        {
            Contract.Requires(client != null);
            if (_objectPoolDict.TryGetValue(client.RemoteName, out ObjectPool<SocketClient> objectPool))
            {
                objectPool.Return(client);
            }
            else
            {
                client.Dispose();
            }
        }
    }
}
