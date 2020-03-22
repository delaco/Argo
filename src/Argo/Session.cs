using System;
using System.Net;
using System.Threading.Tasks;

namespace Argo
{
    public class Session
    {
        private string _sessionId;
        private DateTime _createTime;
        private DateTime _lastAccessTime;
        private EndPoint _remoteAddress;
        private IMessageHandler _messageHandler;

        public string SessionId => _sessionId;

        public DateTime CreateTime => _createTime;

        public DateTime LastAccessTime
        {
            get => _lastAccessTime;
            set => _lastAccessTime = value;
        }

        public EndPoint RemoteAddress => _remoteAddress;

        public IMessageHandler MessageHandler => _messageHandler;

        public virtual void Initialize(EndPoint remoteAddress, IMessageHandler messageHandler)
        {
            _sessionId = Guid.NewGuid().ToString();
            _createTime = DateTime.Now;
            _remoteAddress = remoteAddress;
            _messageHandler = messageHandler;
        }

        public async Task SendAsync(IMessage message)
        {
            await this._messageHandler.SendAsync(message);
        }
    }
}
