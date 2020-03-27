using Argo.Commands;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static DotNetty.Codecs.Http.HttpResponseStatus;
using static DotNetty.Codecs.Http.HttpVersion;

namespace Argo.Internal
{
    public sealed class WebSocketServerHandler : SimpleChannelInboundHandler<object>
    {
        const string WebsocketPath = "/websocket";

        private IServiceProvider _serviceProvider;
        private AppSessionContainer<AppSession> _appSessionContainer;
        private ICommandDescriptorContainer _commandContainer;
        private WebSocketServerHandshaker _handshaker;
        private ICommandActivator _commandActivator;
        private ListenerOptions _netListenerOptions;
        private IPacketCodec _packetCodec;
        private ILogger _logger;

        public WebSocketServerHandler(IServiceProvider serviceProvider, ListenerOptions netListenerOptions)
        {
            this._appSessionContainer = serviceProvider.GetRequiredService<AppSessionContainer<AppSession>>();
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._netListenerOptions = netListenerOptions ?? throw new ArgumentNullException(nameof(netListenerOptions));

            this._commandContainer = serviceProvider.GetRequiredService<ICommandDescriptorContainer>();
            this._commandActivator = serviceProvider.GetRequiredService<ICommandActivator>();
            this._packetCodec = serviceProvider.GetRequiredService<IPacketCodec>();
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<WebSocketServerHandler>();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            if (msg is IFullHttpRequest request)
            {
                this.HandleHttpRequest(ctx, request);
            }
            else if (msg is WebSocketFrame frame)
            {
                this.HandleWebSocketFrame(ctx, frame);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Flush();
        }

        void HandleHttpRequest(IChannelHandlerContext ctx, IFullHttpRequest req)
        {
            // Handle a bad request.
            if (!req.Result.IsSuccess)
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, BadRequest));
                return;
            }

            // Allow only GET methods.
            if (!Equals(req.Method, HttpMethod.Get))
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, Forbidden));
                return;
            }

            // Handshake
            var wsFactory = new WebSocketServerHandshakerFactory(
                GetWebSocketLocation(req), null, true, 5 * 1024 * 1024);
            this._handshaker = wsFactory.NewHandshaker(req);
            if (this._handshaker == null)
            {
                WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            }
            else
            {
                _logger.LogInformation($"New client to handshake:{ctx.Channel}");
                var appSession = new AppSession();
                var messageHandler = new DotNettyMessageHandlerProvider(ctx.Channel, null).Create();
                appSession.Initialize(appSession.RemoteAddress, messageHandler);

                this._appSessionContainer.Set(ctx.Channel.Id.ToString(), appSession);
                this._handshaker.HandshakeAsync(ctx.Channel, req);
            }
        }

        void HandleWebSocketFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            var appSession = _appSessionContainer.Get(ctx.Channel.Id.ToString());
            if (appSession == null)
            {
                throw new ArgumentNullException(nameof(appSession));
            }

            appSession.LastAccessTime = DateTime.Now;
            switch (frame)
            {
                // Check for closing frame
                case CloseWebSocketFrame _:
                    this._handshaker.CloseAsync(ctx.Channel, (CloseWebSocketFrame)frame.Retain());
                    return;
                case PingWebSocketFrame _:
                    ctx.WriteAsync(new PongWebSocketFrame((IByteBuffer)frame.Content.Retain()));
                    return;
                case TextWebSocketFrame _:
                    ctx.WriteAsync(frame.Retain());
                    return;
                case BinaryWebSocketFrame _:
                    HandleBinaryWebSocketFrame((BinaryWebSocketFrame)frame.Retain(), appSession);
                    return;
            }
        }

        private void HandleBinaryWebSocketFrame(BinaryWebSocketFrame binaryWebSocketFrame, AppSession appSession)
        {
            var byteBuffer = binaryWebSocketFrame.Content;
            var readBytes = new byte[byteBuffer.ReadableBytes];
            byteBuffer.ReadBytes(readBytes);
            var message = _packetCodec.Decode(readBytes);

            var requestContext = new RequestContext(appSession, message);
            var commandDescriptor = _commandContainer.Get(requestContext);
            if (commandDescriptor != null)
            {
                var commandContext = new CommandContext(requestContext,
                    commandDescriptor,
                    _serviceProvider);
                if (!(_commandActivator.Create(commandContext) is ICommand command))
                {
                    throw new NotImplementedException();
                }

                command.Execute(requestContext);
            }
            else
            {
                _logger.LogWarning($"The msg' command {byteBuffer} was not found.");
            }
        }

        static void SendHttpResponse(IChannelHandlerContext ctx, IFullHttpRequest req, IFullHttpResponse res)
        {
            // Generate an error page if response getStatus code is not OK (200).
            if (res.Status.Code != 200)
            {
                IByteBuffer buf = Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(res.Status.ToString()));
                res.Content.WriteBytes(buf);
                buf.Release();
                HttpUtil.SetContentLength(res, res.Content.ReadableBytes);
            }

            // Send the response and close the connection if necessary.
            Task task = ctx.Channel.WriteAndFlushAsync(res);
            if (!HttpUtil.IsKeepAlive(req) || res.Status.Code != 200)
            {
                task.ContinueWith((t, c) => ((IChannelHandlerContext)c).CloseAsync(),
                    ctx, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            Console.WriteLine($"{nameof(WebSocketServerHandler)} {0}", e);
            ctx.CloseAsync();
        }

        string GetWebSocketLocation(IFullHttpRequest req)
        {
            bool result = req.Headers.TryGet(HttpHeaderNames.Host, out ICharSequence value);
            Debug.Assert(result, "Host header does not exist.");
            string location = value.ToString() + WebsocketPath;

            if (_netListenerOptions.Ssl)
            {
                return "wss://" + location;
            }
            else
            {
                return "ws://" + location;
            }
        }
    }
}
