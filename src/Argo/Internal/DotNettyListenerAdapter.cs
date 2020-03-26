using DotNetty.Codecs.Http;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Argo.Internal
{
    internal class DotNettyListenerAdapter : INetListener
    {
        private ListenerOptions _options;
        private IServiceProvider _serviceProvider;
        private IChannel _boundChannel;
        private IPacketCodec _packetCodec;

        public DotNettyListenerAdapter(ListenerOptions options, IServiceProvider serviceProvider)
        {
            _options = options;
            _serviceProvider = serviceProvider;
            InternalLoggerFactory.DefaultFactory = _serviceProvider.GetService<ILoggerFactory>();
            _packetCodec = _serviceProvider.GetRequiredService<IPacketCodec>();
        }

        public async Task StartAsync()
        {
          
            IEventLoopGroup workerGroup = new MultithreadEventLoopGroup();

            if (_options.ProtocolType == ProtocolType.Tcp)
            {
                IEventLoopGroup bossGroup = new MultithreadEventLoopGroup(1);
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>();

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
                           pipeline.AddLast("framing-enc", new DotNettyMessageEncoder(_packetCodec));
                           pipeline.AddLast("framing-dec", new DotNettyMessageDecoder(_packetCodec));
                           pipeline.AddLast(new SocketServerHandler<PacketInfo>(_serviceProvider, true));
                       }
                       else // websocket support
                       {
                           pipeline.AddLast(new HttpServerCodec());
                           pipeline.AddLast("framing-enc", new DotNettyMessageWebSocketEncoder(_packetCodec));
                           pipeline.AddLast(new HttpObjectAggregator(65536));
                           pipeline.AddLast(new WebSocketServerHandler(_serviceProvider, _options));
                       }
                   }));
                _boundChannel = await bootstrap.BindAsync(_options.Port);
            }
            else
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(workerGroup)
                    .Channel<SocketDatagramChannel>();
                _boundChannel = await bootstrap.BindAsync(_options.Port);
            }

          
        }

        public async Task CloseAsync()
        {
            await _boundChannel?.CloseAsync();
        }
    }
}