using DotNetty.Transport.Channels;

namespace Argo
{
    public class DotNettyMessageHandlerFactory : IMessageHandlerFactory
    {
        private IChannel _channel;
        private ClientWaits _clientWaits;

        public DotNettyMessageHandlerFactory(IChannel channel, ClientWaits clientWaits)
        {
            this._channel = channel;
            this._clientWaits = clientWaits;
        }

        public IMessageHandler Create()
        {
            return new DotNettyMessageHandler(_channel, _clientWaits);
        }
    }
}
