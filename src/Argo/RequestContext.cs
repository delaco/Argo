namespace Argo
{
    public class RequestContext
    {
        private IMessage _message;
        private Session _session;

        /// <summary>
        /// Gets the message
        /// </summary>
        /// <returns></returns>
        public IMessage Message => _message;

        /// <summary>
        /// Gets the session
        /// </summary>
        /// <returns></returns>
        public Session Session => _session;

        public RequestContext(Session session, IMessage message)
        {
            this._session = session;
            this._message = message;
        }
    }
}