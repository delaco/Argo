namespace Argo
{
    public class RequestContext
    {
        private IPacket _packet;
        private AppSession _appSession;

        /// <summary>
        /// Gets the request packet
        /// </summary>
        /// <returns></returns>
        public IPacket Request => _packet;

        /// <summary>
        /// Gets the appSession
        /// </summary>
        /// <returns></returns>
        public AppSession AppSession => _appSession;

        public RequestContext(AppSession appSession, IPacket packet)
        {
            this._appSession = appSession;
            this._packet = packet;
        }
    }
}