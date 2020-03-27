using DotNetty.Transport.Channels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Argo.Internal
{
    public class DotNettyMessageHandler : IMessageHandler
    {
        private IChannel _channel;
        private ClientWaits _clientWait;
        public readonly AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

        public IPacket AyscResponse { get; set; }

        internal DotNettyMessageHandler(IChannel channel, ClientWaits clientWait)
        {
            this._channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this._clientWait = clientWait;
        }

        public async Task SendAsync(IPacket message)
        {
            await _channel.WriteAndFlushAsync(message).ConfigureAwait(false);
        }

        public Task<IPacket> Send(IPacket message)
        {
            if (_clientWait == null)
                throw new ArgumentNullException(nameof(_clientWait));

            return Task.Run(() =>
            {
                this.AyscResponse = null;
                var key = _channel.Id.ToString();
                _clientWait.Start(key, this);
                _channel.WriteAndFlushAsync(message);
                _clientWait.Wait(key);

                if (this.AyscResponse == null)
                {
                    throw new TimeoutException($"Send to remote server timeout:{_channel}");
                }

                return this.AyscResponse;
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
