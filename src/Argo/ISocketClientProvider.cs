namespace Argo
{
    public interface ISocketClientProvider
    {
        SocketClient Create(SocketClientOptions option);
    }
}
