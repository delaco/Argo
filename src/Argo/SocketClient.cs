using System.Threading.Tasks;
using System;

namespace Argo
{
    public class SocketClient : IDisposable
    {
        private IMessageHandler _messageHandler;

        public string RemoteName { get; }


        public SocketClient(string remoteName, IMessageHandlerProvider messageHandlerFactory)
        {
            this.RemoteName = remoteName;
            this._messageHandler = messageHandlerFactory.Create();
        }

        public async Task SendAsync(IPacket message)
        {
            await _messageHandler.SendAsync(message);
        }

        public Task<IPacket> Send(IPacket message)
        {
            return _messageHandler.Send(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_messageHandler != null)
                {
                    _messageHandler.Dispose();
                    _messageHandler = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
