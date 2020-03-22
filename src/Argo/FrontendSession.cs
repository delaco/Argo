using System.Net;
using System.Threading.Tasks;

namespace Argo
{
    /// <summary>
    /// todo:
    /// </summary>
    public class FrontendSession
    {
        public Session Session { get; }

        public string UserId { get; }

        public string Host { get; }

        public async Task SendAsync(IMessage message)
        {
            await Session.SendAsync(message);
        }
    }
}
