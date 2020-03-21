using System.Collections.Generic;
using System.Net.Sockets;

namespace Argo
{
    public class RemoteOptions
    {
        public List<SocketClientOptions> Remotes { get; set; }
    }

    public class SocketClientOptions
    {
        public string Name { get; set; }

        public ProtocolType ProtocolType { get; set; } = ProtocolType.Tcp;

        public string Host { get; set; }

        public int Port { get; set; }

        public int PoolSize { get; set; } = 5;
    }
}