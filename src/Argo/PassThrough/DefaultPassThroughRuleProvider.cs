namespace Argo.PassThrough
{
    public class DefaultPassThroughRuleProvider : IPassThroughRuleProvider
    {
        public IPassThroughRule Create(RequestContext requestContext)
        {
            return new NullPassThroughRule(requestContext);
        }
    }
}