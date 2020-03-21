using DotNetty.Common.Utilities;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Argo
{
    public class DottNettyClientAdapter : ISocketClientProvider
    {
        private RemoteOptions _options;
        private IServiceProvider _serviceProvider;


        public DottNettyClientAdapter(RemoteOptions options, IServiceProvider serviceProvider)
        {
            this._options = options ?? throw new ArgumentNullException(nameof(options));
            _serviceProvider = serviceProvider;
        }

        public SocketClient Create(string connectionName)
        {
            var group = new MultithreadEventLoopGroup();
            var option = _options.Remotes.Find(v => v.Name == connectionName);
            if (option == null)
            {
                throw new IndexOutOfRangeException(nameof(_options));
            }

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
                       pipeline.AddLast("framing-enc", new HeaderPrepender());
                       pipeline.AddLast("framing-dec", new HeaderBasedFrameDecoder());
                       pipeline.AddLast(new SyncReceiverHandler<IMessage>(_serviceProvider, true));
                       //pipeline.AddLast(new ReceiverHandler<IMessage>(_serviceProvider, true));
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
            var tcpClient = new SocketClient(option.Name, new DotNettyMessageHandlerFactory(bootstrapChannel, clientWait));

            return tcpClient;
        }

        internal class ReceiverHandler<T> : ChannelHandlerAdapter
             where T : IMessage
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly bool _autoRelease;

            public IServiceProvider ServiceProvider => _serviceProvider;

            public ReceiverHandler(IServiceProvider serviceProvider, bool autoRelease)
            {
                this._serviceProvider = serviceProvider;
                this._autoRelease = autoRelease;
            }

            bool AcceptInboundMessage(object msg) => msg is IMessage;

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
          where T : IMessage
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly bool _autoRelease;
            private ClientWaits _clientWaits;

            public IServiceProvider ServiceProvider => _serviceProvider;

            public SyncReceiverHandler(IServiceProvider serviceProvider, bool autoRelease)
            {
                this._serviceProvider = serviceProvider;
                this._autoRelease = autoRelease;
                _clientWaits = serviceProvider.GetRequiredService<ClientWaits>();
            }

            bool AcceptInboundMessage(object msg) => msg is IMessage;

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
                        var message = (IMessage)msg;
                        _clientWaits.Set(context.Channel.Id.AsShortText(), message);
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
