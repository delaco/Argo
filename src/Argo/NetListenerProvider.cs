using Argo.Internal;
using System;

namespace Argo
{
    public class NetListenerProvider : INetListenerProvider
    {
        private IServiceProvider _serviceProvider;

        public NetListenerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        ///  Create an <see cref="INetListener">.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public INetListener CreateListener(NetListenerOptions options)
        {
            return new DotNettyListenerAdapter(options, _serviceProvider);
        }
    }
}
