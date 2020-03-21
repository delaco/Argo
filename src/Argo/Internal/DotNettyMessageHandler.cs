using DotNetty.Transport.Channels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Argo.Internal
{
    internal class DotNettyMessageHandler : IMessageHandler
    {
        public AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

        private IChannel _channel;
        private ClientWaits _clientWait;

        public IMessage Response { get; set; }

        internal DotNettyMessageHandler(IChannel channel, ClientWaits clientWait)
        {
            this._channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this._clientWait = clientWait;
        }

        public async Task SendAsync(IMessage message)
        {
            await _channel.WriteAndFlushAsync(message);
        }

        public Task<IMessage> Send(IMessage message)
        {
            return Task.Run(() =>
            {
                this.Response = null;
                var key = _channel.Id.ToString();
                _clientWait.Start(key, this);
                _channel.WriteAndFlushAsync(message);
                _clientWait.Wait(key);

                if (this.Response == null)
                {
                    throw new TimeoutException($"Send to remote server timeout:{_channel}");
                }

                return this.Response;
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_channel != null)
                {
                    _channel.CloseAsync().GetAwaiter().GetResult();
                    _channel = null;
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
