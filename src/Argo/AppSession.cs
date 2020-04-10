using Argo.Internal;
using DotNetty.Transport.Channels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Argo
{
    public class AppSession : ISession
    {
        private IChannel _channel;

        public string Id => _channel.Id.ToString();

        public DateTime CreateTime { get; private set; }

        public DateTime LastAccessTime { get; set; }

        public EndPoint RemoteAddress { get; private set; }

        public IMessageHandler MessageHandler { get; private set; }

        public void Initialize(IChannel channel, EndPoint remoteAddress)
        {
            _channel = channel;
            CreateTime = DateTime.Now;
            RemoteAddress = remoteAddress;
            MessageHandler = new DotNettyMessageHandlerProvider(channel).Create();
        }

        public async Task SendAsync(IPacket message)
        {
            await MessageHandler.SendAsync(message);
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
