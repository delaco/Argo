namespace Argo
{
    public abstract class ClientMessageRouter : IMessageRouter
    {
        public abstract void Route(RequestContext requestContext);
    }
}
