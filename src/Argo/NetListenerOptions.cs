using System.Collections.Generic;
using System.Net.Sockets;

namespace Argo
{
    public class NetServerOptions
    {
        public List<NetListenerOptions> ListenerOptions { get; set; }
    }

    public class NetListenerOptions
    {
        public SocketMode SocketMode { get; set; }

        public ProtocolType ProtocolType { get; set; }

        public int Port { get; set; }

        public bool Ssl { get; set; }

        public List<string> CommandAssemblies { get; set; }
    }
}
