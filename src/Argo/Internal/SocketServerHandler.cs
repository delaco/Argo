using Argo.Commands;
using DotNetty.Common.Utilities;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Sockets;

namespace Argo.Internal
{
    public sealed class SocketServerHandler<T> : ChannelHandlerAdapter
              where T : IPacket
    {
        private IServiceProvider _serviceProvider;
        private SessionContainer<Session> _sessionContainer;
        private IMessageRouter _messageRouter;
        private readonly bool _autoRelease;
        private ILogger _logger;
        private ICommandDescriptorContainer _commandContainer;
        private ICommandActivator _commandActivator;


        public SocketServerHandler(IServiceProvider serviceProvider, bool autoRelease)
        {
            this._sessionContainer = serviceProvider.GetRequiredService<SessionContainer<Session>>();

            this._messageRouter = serviceProvider.GetRequiredService<IMessageRouter>();
            this._commandContainer = serviceProvider.GetRequiredService<ICommandDescriptorContainer>();
            this._commandActivator = serviceProvider.GetRequiredService<ICommandActivator>();
            this._serviceProvider = serviceProvider;
            this._autoRelease = autoRelease;
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<SocketServerHandler<T>>();
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _logger.LogInformation($"A new connection from:{context.Channel.LocalAddress}");
            base.ChannelActive(context);

            var channel = context.Channel;
            var session = new Session();
            var messageHandler = new DotNettyMessageHandlerProvider(channel, null).Create();
            session.Initialize(channel.RemoteAddress, messageHandler);

            this._sessionContainer.Set(channel.Id.ToString(), session);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            this._sessionContainer.Remove(context.Channel.Id.ToString());
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            //time out can not trigger
            if (evt is IdleStateEvent idleStateEvent)
            {
                switch (idleStateEvent.State)
                {
                    case IdleState.AllIdle:
                    case IdleState.ReaderIdle:
                    case IdleState.WriterIdle:
                        context.CloseAsync();
                        break;
                }
            }

            base.UserEventTriggered(context, evt);
        }

        public override void ChannelRead(IChannelHandlerContext context, object msg)
        {
            var channel = context.Channel;
            var session = _sessionContainer.Get(channel.Id.ToString());
            if (session != null)
            {
                session.LastAccessTime = DateTime.Now;
            }

          
            bool release = true;
            try
            {
                if (msg is IPacket packet)
                {
                    _logger.LogInformation($"The msg {msg} from {channel} has been read.");
                    var requestContext = new RequestContext(session, packet);
                    _messageRouter.Route(requestContext);
                }
                else
                {
                    _logger.LogInformation($"The msg {msg} is not type of IMessage.");
                    release = false;
                    context.FireChannelRead(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ChannelRead error", ex);
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

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception is SocketException socketException)
            {
                if (socketException.SocketErrorCode == SocketError.ConnectionAborted ||
                    socketException.SocketErrorCode == SocketError.ConnectionReset)
                {
                    _logger.LogError(new EventId(0), socketException, socketException.Message);
                    // todo:
                }
            }

            _logger.LogError(exception, exception.Message);
        }
    }
}
