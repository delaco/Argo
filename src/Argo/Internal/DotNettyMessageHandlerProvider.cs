using DotNetty.Transport.Channels;

namespace Argo.Internal
{
    internal class DotNettyMessageHandlerProvider : IMessageHandlerProvider
    {
        private IChannel _channel;
        private ClientWaits _clientWaits;

        public DotNettyMessageHandlerProvider(IChannel channel, ClientWaits clientWaits)
        {
            this._channel = channel;
            this._clientWaits = clientWaits;
        }

        public DotNettyMessageHandlerProvider(IChannel channel)
        {
            this._channel = channel;
        }

        public IMessageHandler Create()
        {
            return new DotNettyMessageHandler(_channel, _clientWaits);
        }
    }
}
