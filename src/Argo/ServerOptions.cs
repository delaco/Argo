using System.Collections.Generic;
using System.Net.Sockets;

namespace Argo
{
    public class ServerOptions
    {
        public string ServerName { get; set; }

        /// <summary>
        /// 开启异常会话清理
        /// </summary>
        public bool InvalidSessionRelease { get; set; } = false;

        /// <summary>
        /// Monitor执行间隔
        /// </summary>
        public int MonitorInterval { get; set; }

        /// <summary>
        /// 会话失效时间（s）
        /// </summary>
        public int SessionDeadTime { get; set; } = 3600;

        public List<ListenerOptions> Listeners { get; set; }
    }

    public class ListenerOptions
    {
        public SocketMode SocketMode { get; set; }

        public ProtocolType ProtocolType { get; set; }

        public int Port { get; set; }

        public bool Ssl { get; set; }
    }
}
