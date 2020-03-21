namespace Argo.PassThrough
{
    public interface IPassThroughRuleProvider
    {
        IPassThroughRule Create(RequestContext requestContext);
    }
}