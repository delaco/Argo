namespace Argo.PassThrough
{
    internal class DefaultPassThroughRuleProvider : IPassThroughRuleProvider
    {
        public IPassThroughRule Create(RequestContext requestContext)
        {
            return new NullPassThroughRule(requestContext);
        }
    }
}