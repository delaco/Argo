using Argo.Internal;
using System;

namespace Argo.Internal
{
    public class DotNettyListenerProvider : INetListenerProvider
    {
        private IServiceProvider _serviceProvider;

        public DotNettyListenerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        ///  Create an <see cref="INetListener">.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public INetListener CreateListener(ListenerOptions options)
        {
            return new DotNettyListenerAdapter(options, _serviceProvider);
        }
    }
}
