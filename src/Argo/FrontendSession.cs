using System.Net;

namespace Argo
{
    /// <summary>
    /// todo:
    /// </summary>
    public class FrontendSession : Session
    {
        public string UserId { get; }

        public EndPoint ConnectionAddress { get; }
    }
}
