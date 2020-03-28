namespace Argo.PassThrough
{
    public class PassThroughRule : IPassThroughRule
    {
        private RequestContext _requestContext;
        public RequestContext RequestContext => _requestContext;

        public bool  IsPassThrough => false;

        public PassThroughRule(RequestContext requestContext)
        {
            _requestContext = requestContext;
        }
    }
}