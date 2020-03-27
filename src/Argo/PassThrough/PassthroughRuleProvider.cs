namespace Argo.PassThrough
{
    public class PassThroughRuleProvider : IPassThroughRuleProvider
    {
        public IPassThroughRule Create(RequestContext requestContext)
        {
            return new PassThroughRule(requestContext);
        }
    }
}