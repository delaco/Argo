namespace Argo
{
    public interface IMessageRouter
    {
        void Route(RequestContext requestContext);
    }
}