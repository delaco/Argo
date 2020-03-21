namespace Argo
{
    public interface IMessageHandlerProvider
    {
        IMessageHandler Create();
    }
}
