using System.Threading;

namespace Argo
{
    public class ClientObject
    {
        public AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

        public IMessage Response { get; set; }
    }
}
