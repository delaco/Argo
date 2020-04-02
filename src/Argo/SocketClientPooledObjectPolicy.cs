using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Argo
{
    public class SocketClientPooledObjectPolicy : IPooledObjectPolicy<SocketClient>
    {
        private ISocketClientProvider _socketClientProvider;
        private ClientOptions _socketClientOptions;
        public SocketClientPooledObjectPolicy(ClientOptions socketClientOptions, ISocketClientProvider socketClientProvider)
        {
            _socketClientProvider = socketClientProvider ?? throw new ArgumentNullException(nameof(socketClientProvider));
            _socketClientOptions = socketClientOptions ?? throw new ArgumentNullException(nameof(socketClientOptions));
        }

        public SocketClient Create()
        {
            return _socketClientProvider.Create(_socketClientOptions);
        }

        public bool Return(SocketClient obj)
        {
            return true;
        }
    }
}
