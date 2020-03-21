using System.Threading.Tasks;

namespace Argo
{
    public class SocketClient
    {
        public IMessageHandler MessageHandler { get; }

        public string ConnectionName { get; }


        public SocketClient(string connectionName, IMessageHandlerFactory messageHandlerFactory)
        {
            this.ConnectionName = connectionName;
            this.MessageHandler = messageHandlerFactory.Create();
        }

        public async Task SendAsync(IMessage message)
        {
            await MessageHandler.SendAsync(message);
        }

        public IMessage Send(IMessage message)
        {
            return MessageHandler.Send(message);
        }
    }
}
