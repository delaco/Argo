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
        static readonly AttributeKey<Session> SessionKey = AttributeKey<Session>.ValueOf(nameof(Session));

        private IServiceProvider _serviceProvider;
        private SessionContainer<Session> _sessionContainer;
        private ICommandDescriptorContainer _commandContainer;
        private WebSocketServerHandshaker _handshaker;
        private ICommandActivator _commandActivator;
        private ListenerOptions _netListenerOptions;
        private IMessageCodec _messageCodec;
        private ILogger _logger;

        public WebSocketServerHandler(IServiceProvider serviceProvider, ListenerOptions netListenerOptions)
        {
            this._sessionContainer = serviceProvider.GetRequiredService<SessionContainer<Session>>();
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._netListenerOptions = netListenerOptions ?? throw new ArgumentNullException(nameof(netListenerOptions));

            this._commandContainer = serviceProvider.GetRequiredService<ICommandDescriptorContainer>();
            this._commandActivator = serviceProvider.GetRequiredService<ICommandActivator>();
            this._messageCodec = serviceProvider.GetRequiredService<IMessageCodec>();
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<WebSocketServerHandler>();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
        {
            if (msg is IFullHttpRequest request)
            {
                this.HandleHttpRequest(ctx, request);
            }
            else if (msg is WebSocketFrame frame)
            {
                this.HandleWebSocketFrame(ctx, frame);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

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
                var channel = ctx.Channel;
                var session = new Session();
                var messageHandler = new DotNettyMessageHandlerProvider(channel, null).Create();
                session.Initialize(channel.RemoteAddress, messageHandler);

                ctx.Channel.GetAttribute(SessionKey).Set(session);
                this._sessionContainer.Set(channel.Id.ToString(), session);
                this._handshaker.HandshakeAsync(ctx.Channel, req);
            }
        }

        void HandleWebSocketFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
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
                    HandleBinaryWebSocketFrame(ctx, (BinaryWebSocketFrame)frame.Retain());
                    return;
            }
        }

        void HandleBinaryWebSocketFrame(IChannelHandlerContext context, BinaryWebSocketFrame binaryWebSocketFrame)
        {
            var byteBuffer = binaryWebSocketFrame.Content;
            var readBytes = new byte[byteBuffer.ReadableBytes];
            byteBuffer.ReadBytes(readBytes);
            var message =_messageCodec.Decode(readBytes);

            var session = context.Channel.GetAttribute(SessionKey).Get();
            session.LastAccessTime = DateTime.Now;
            var requestContext = new RequestContext(session, message);
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

        static long GetFrameValue(IByteBuffer buffer, int offset, int length)
        {
            long frameLength;
            switch (length)
            {
                case 1:
                    frameLength = buffer.GetByte(offset);
                    break;
                case 2:
                    frameLength = buffer.GetUnsignedShortLE(offset);
                    break;
                case 3:
                    frameLength = buffer.GetUnsignedMediumLE(offset);
                    break;
                case 4:
                    frameLength = buffer.GetIntLE(offset);
                    break;
                case 8:
                    frameLength = buffer.GetLongLE(offset);
                    break;
                default:
                    throw new DecoderException("unsupported lengthFieldLength: " + length + " (expected: 1, 2, 3, 4, or 8)");
            }

            return frameLength;
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
