namespace Argo.PassThrough
{
    public class PassThroughRule : IPassThroughRule
    {
        private RequestContext _requestContext;
        public RequestContext RequestContext => _requestContext;

        bool IPassThroughRule.IsPassThrough => false;

        public PassThroughRule(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        public bool Matches()
        {
            return false;
        }
    }
}