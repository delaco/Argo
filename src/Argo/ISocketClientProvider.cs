namespace Argo
{
    public interface ISocketClientProvider
    {
        SocketClient Create(ClientOptions socketClientOptions);
    }
}
