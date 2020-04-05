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
        /// <param name="packet"></param>
        /// <returns></returns>
        Task SendAsync(IPacket packet);

        /// <summary>
        /// Send and recevie
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        Task<IPacket> SendAndRecevieAsync(IPacket packet);
    }
}
