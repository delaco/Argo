namespace Argo
{
    public interface ISocketClientProvider
    {
        SocketClient Create(string connectionName);
    }
}
