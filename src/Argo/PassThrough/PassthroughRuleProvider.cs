namespace Argo.PassThrough
{
    internal class PassThroughRuleProvider : IPassThroughRuleProvider
    {
        public IPassThroughRule Create(RequestContext requestContext)
        {
            return new PassThroughRule(requestContext);
        }
    }
}