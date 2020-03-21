namespace Argo
{
    public class RequestContext
    {
        private MessagePacket _message;
        private Session _session;

        /// <summary>
        /// Gets the message pack
        /// </summary>
        /// <returns></returns>
        public MessagePacket Message => _message;

        /// <summary>
        /// Gets the session
        /// </summary>
        /// <returns></returns>
        public Session Session => _session;

        public RequestContext(Session session, MessagePacket message)
        {
            this._session = session;
            this._message = message;
        }
    }
}