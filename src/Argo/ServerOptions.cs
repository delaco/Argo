using System.Collections.Generic;
using System.Net.Sockets;

namespace Argo
{
    public class ServerOptions
    {
        public string ServerName { get; set; }

        public List<ListenerOptions> Listeners { get; set; }
    }

    public class ListenerOptions
    {
        public SocketMode SocketMode { get; set; }

        public ProtocolType ProtocolType { get; set; }

        public int Port { get; set; }

        public bool Ssl { get; set; }

        public List<string> CommandAssemblies { get; set; }
    }
}
