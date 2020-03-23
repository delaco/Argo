using System;
using System.Threading;
using System.Threading.Tasks;

namespace Argo
{
    /// <summary>
    /// Represents a handler that send message.
    /// </summary>
    public interface IMessageHandler : IDisposable
    {
        /// <summary>
        /// SendAsync
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendAsync(IPacket message);

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<IPacket> Send(IPacket message);
    }
}
