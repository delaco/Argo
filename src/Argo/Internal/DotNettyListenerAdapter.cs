using DotNetty.Codecs.Http;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Argo.Internal
{
    internal class DotNettyListenerAdapter : INetListener
    {
        private NetListenerOptions _options;
        private IServiceProvider _serviceProvider;
        private IChannel _boundChannel;

        public DotNettyListenerAdapter(NetListenerOptions options, IServiceProvider serviceProvider)
        {
            _options = options;
            _serviceProvider = serviceProvider;
            InternalLoggerFactory.DefaultFactory = _serviceProvider.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
        }

        public async Task StartListenerAsync()
        {
            IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
            IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();

            dynamic bootstrap;
            if (_options.ProtocolType == ProtocolType.Tcp)
            {
                bootstrap = new ServerBootstrap();
                bootstrap.Group(bossGroup, workerGroup);
                bootstrap.Channel<TcpServerSocketChannel>();

                bootstrap
                   .Option(ChannelOption.SoBacklog, 8192)
                   .Handler(new LoggingHandler("SRV-LSTN"))
                   .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                   {
                       IChannelPipeline pipeline = channel.Pipeline;
                       if (_options.Ssl)
                       {
                           var tlsCertificate = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dotnetty.com.pfx"), "password");
                           pipeline.AddLast("tls", TlsHandler.Server(tlsCertificate));
                       }

                       pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                       pipeline.AddLast(new IdleStateHandler(TimeSpan.FromHours(8), TimeSpan.FromHours(8), TimeSpan.FromHours(8)));
                       if (_options.SocketMode == SocketMode.Socket)
                       {
                           pipeline.AddLast("framing-enc", new HeaderPrepender());
                           pipeline.AddLast("framing-dec", new HeaderBasedFrameDecoder());
                           pipeline.AddLast(new SocketServerHandler<MessagePacket>(_serviceProvider, true));
                       }
                       else // websocket support
                       {
                           pipeline.AddLast(new HttpServerCodec());
                           pipeline.AddLast(new HttpObjectAggregator(65536));
                           pipeline.AddLast(new WebSocketServerHandler(_serviceProvider, _options));
                       }
                   }));
            }
            else
            {
                bootstrap = new Bootstrap();
                bootstrap
                    .Group(workerGroup)
                    .Channel<SocketDatagramChannel>();
            }

            _boundChannel = await bootstrap.BindAsync(_options.Port);
        }

        public async Task CloseListenerAsync()
        {
            await _boundChannel?.CloseAsync();
        }
    }
}