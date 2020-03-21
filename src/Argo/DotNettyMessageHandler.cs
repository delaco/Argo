using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;

namespace Argo
{
    public class DotNettyMessageHandler : IMessageHandler
    {
        private bool disposedValue = false;
        private IChannel _channel;
        private ClientWaits _clientWait;
        internal DotNettyMessageHandler(IChannel channel, ClientWaits clientWait)
        {
            this._channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this._clientWait = clientWait;
        }

        public async Task SendAsync(IMessage message)
        {
            await _channel.WriteAndFlushAsync(message);
        }

        public IMessage Send(IMessage message)
        {
            var key = _channel.Id.AsShortText();
            _clientWait.Start(key);
            _channel.WriteAndFlushAsync(message);
            var resp = _clientWait.Wait(key).Response;

            return resp;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                if (_channel != null)
                {
                    _channel.CloseAsync().GetAwaiter().GetResult();
                    _channel = null;
                }

                disposedValue = true;
            }
        }

        ~DotNettyMessageHandler()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
