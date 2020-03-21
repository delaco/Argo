using System;
using System.Collections.Concurrent;

namespace Argo
{
    public interface ISocketClientPoolContainer
    {
        SocketClient Get(string connectionName);

        void Release(SocketClient client);
    }

    public class SocketClientPoolContainer : ISocketClientPoolContainer
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<SocketClient>> _clientsDict
            = new ConcurrentDictionary<string, ConcurrentQueue<SocketClient>>();

        private ISocketClientProvider _socketClientProvider;

        public SocketClientPoolContainer(ISocketClientProvider socketClientProvider)
        {
            _socketClientProvider = socketClientProvider;
        }

        public SocketClient Get(string connectionName)
        {
            SocketClient client = default;
            if (_clientsDict.TryGetValue(connectionName, out ConcurrentQueue<SocketClient> clientQueue))
            {
                while (true)
                {
                    clientQueue.TryDequeue(out client);

                    if (client == null)
                        break;
                }
            }
            else
            {
                clientQueue = new ConcurrentQueue<SocketClient>();
                _clientsDict.TryAdd(connectionName, clientQueue);
            }

            return client ?? Create(connectionName);
        }

        public void Release(SocketClient client)
        {
            var connectionName = client.ConnectionName;
            if (!_clientsDict.TryGetValue(connectionName, out ConcurrentQueue<SocketClient> clientQueue))
            {
                clientQueue = new ConcurrentQueue<SocketClient>();
                _clientsDict.TryAdd(connectionName, clientQueue);
            }

            clientQueue.Enqueue(client);
        }

        private SocketClient Create(string connectionName)
        {
            var client = _socketClientProvider.Create(connectionName);
            if (client == null)
            {
                throw new Exception($"The {connectionName} failed to create the client");
            }

            return client;
        }
    }
}
