using System;
using System.Net;
using System.Threading.Tasks;

namespace Argo
{
    public class AppSession : ISession
    {
        private string _id;
        private DateTime _createTime;
        private DateTime _lastAccessTime;
        private EndPoint _remoteAddress;
        private IMessageHandler _messageHandler;

        public string Id => _id;

        public DateTime CreateTime => _createTime;

        public DateTime LastAccessTime
        {
            get => _lastAccessTime;
            set => _lastAccessTime = value;
        }

        public EndPoint RemoteAddress => _remoteAddress;

        public IMessageHandler MessageHandler => _messageHandler;

        public virtual void Initialize(string id, EndPoint remoteAddress, IMessageHandler messageHandler)
        {
            _id = id;
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
    }
}
