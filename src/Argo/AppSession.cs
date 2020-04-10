using DotNetty.Transport.Channels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Argo
{
    public class AppSession : ISession
    {
        private DateTime _createTime;
        private DateTime _lastAccessTime;
        private EndPoint _remoteAddress;
        private IMessageHandler _messageHandler;
        private IChannel _channel;

        public string Id => _channel.Id.ToString();

        public DateTime CreateTime => _createTime;

        public DateTime LastAccessTime
        {
            get => _lastAccessTime;
            set => _lastAccessTime = value;
        }

        public EndPoint RemoteAddress => _remoteAddress;

        public IMessageHandler MessageHandler => _messageHandler;

        public void Initialize(IChannel channel, EndPoint remoteAddress, IMessageHandler messageHandler)
        {
            _channel = channel;
            _createTime = DateTime.Now;
            _remoteAddress = remoteAddress;
            _messageHandler = messageHandler;
        }

        public async Task SendAsync(IPacket message)
        {
            await _messageHandler.SendAsync(message).ConfigureAwait(false);
        }

        public override string ToString()
        {
            return $"Id:{Id} CreateTime:{CreateTime} LastAccessTime:{LastAccessTime} {RemoteAddress}";
        }

        public async Task CloseAsync()
        {
            await _channel.CloseAsync();
        }
    }
}
