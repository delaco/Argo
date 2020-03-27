using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;

namespace Argo.Internal
{
    public class DottNettyClientAdapter : ISocketClientProvider
    {
        private ILogger<DottNettyClientAdapter> _logger;
        private IServiceProvider _serviceProvider;
        private IPacketCodec _packetCodec;

        public DottNettyClientAdapter(ILogger<DottNettyClientAdapter> logger, IServiceProvider serviceProvider)
        {
            this._logger = logger;
            _serviceProvider = serviceProvider;
            _packetCodec = _serviceProvider.GetRequiredService<IPacketCodec>();
        }

        public SocketClient Create(SocketClientOptions option)
        {
            Contract.Requires(option != null);
            var group = new MultithreadEventLoopGroup();

            Bootstrap bootstrap;
            if (option.ProtocolType == ProtocolType.Tcp)
            {
                bootstrap = new Bootstrap();
                bootstrap
                   .Group(group)
                   .Channel<TcpSocketChannel>()
                   .Option(ChannelOption.TcpNodelay, true)
                   .Handler(new ActionChannelInitializer<IChannel>(channel =>
                   {
                       IChannelPipeline pipeline = channel.Pipeline;
                       pipeline.AddLast(new LoggingHandler("CONN"));
                       pipeline.AddLast("framing-enc", new DotNettyMessageEncoder(_packetCodec));
                       pipeline.AddLast("framing-dec", new DotNettyMessageDecoder(_packetCodec));
                       pipeline.AddLast(new SyncReceiverHandler<IPacket>(_serviceProvider, true));
                       pipeline.AddLast(new ReceiverHandler<IPacket>(_serviceProvider, true));
                   }));
            }
            else
            {
                throw new NotImplementedException();
            }

            var bootstrapChannel = bootstrap
                .ConnectAsync(new IPEndPoint(IPAddress.Parse(option.Host), option.Port))
                .GetAwaiter()
                .GetResult();
            var clientWait = _serviceProvider.GetRequiredService<ClientWaits>();
            var tcpClient = new SocketClient(option.Name, new DotNettyMessageHandlerProvider(bootstrapChannel, clientWait));
            this._logger.LogInformation($"Create new client channel:{ bootstrapChannel}");

            return tcpClient;
        }

        internal class ReceiverHandler<T> : ChannelHandlerAdapter
             where T : IPacket
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly bool _autoRelease;

            public IServiceProvider ServiceProvider => _serviceProvider;

            public ReceiverHandler(IServiceProvider serviceProvider, bool autoRelease)
            {
                this._serviceProvider = serviceProvider;
                this._autoRelease = autoRelease;
            }

            bool AcceptInboundMessage(object msg) => msg is IPacket;

            public override void ChannelInactive(IChannelHandlerContext context)
            {
                base.ChannelInactive(context);
            }

            public override void ChannelRead(IChannelHandlerContext context, object msg)
            {
                bool release = true;
                try
                {
                    if (this.AcceptInboundMessage(msg))
                    {
                    }
                    else
                    {
                        release = false;
                        context.FireChannelRead(msg);
                    }
                }
                finally
                {
                    if (_autoRelease && release)
                    {
                        ReferenceCountUtil.Release(msg);
                    }
                }
            }

            public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        }

        internal class SyncReceiverHandler<T> : ChannelHandlerAdapter
          where T : IPacket
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly bool _autoRelease;
            private ClientWaits _clientWaits;

            public SyncReceiverHandler(IServiceProvider serviceProvider, bool autoRelease)
            {
                this._serviceProvider = serviceProvider;
                this._autoRelease = autoRelease;
                _clientWaits = serviceProvider.GetRequiredService<ClientWaits>();
            }

            public IServiceProvider ServiceProvider => _serviceProvider;

            public override void ChannelInactive(IChannelHandlerContext context)
            {
                base.ChannelInactive(context);
            }

            public override void ChannelRead(IChannelHandlerContext context, object msg)
            {
                bool release = true;
                try
                {
                    if (msg is IPacket message && message.Sequence == 0)
                    {
                        _clientWaits.Set(context.Channel.Id.ToString(), message);
                    }
                    else
                    {
                        release = false;
                        context.FireChannelRead(msg);
                    }
                }
                finally
                {
                    if (_autoRelease && release)
                    {
                        ReferenceCountUtil.Release(msg);
                    }
                }

            }

            public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        }
    }
}
