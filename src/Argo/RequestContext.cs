namespace Argo
{
    public class RequestContext
    {
        private IPacket _packet;
        private Session _session;

        /// <summary>
        /// Gets the packet
        /// </summary>
        /// <returns></returns>
        public IPacket Packet => _packet;

        /// <summary>
        /// Gets the session
        /// </summary>
        /// <returns></returns>
        public Session Session => _session;

        public RequestContext(Session session, IPacket packet)
        {
            this._session = session;
            this._packet = packet;
        }
    }
}