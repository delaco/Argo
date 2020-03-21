namespace Argo.PassThrough
{
    internal class NullPassThroughRule : IPassThroughRule
    {
        private RequestContext _requestContext;
        public RequestContext RequestContext => _requestContext;

        bool IPassThroughRule.IsPassThrough => false;

        public NullPassThroughRule(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }
    }
}