namespace Argo
{
    public class RequestContext
    {
        /// <summary>
        /// Gets the request packet
        /// </summary>
        /// <returns></returns>
        public IPacket Request { get; set; }

        /// <summary>
        /// Gets the appSession
        /// </summary>
        /// <returns></returns>
        public AppSession AppSession { get; set; }

        public RequestContext()
        {
        }
    }
}