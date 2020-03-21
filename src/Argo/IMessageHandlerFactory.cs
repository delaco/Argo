namespace Argo
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler Create();
    }
}
