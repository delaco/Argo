using Argo.AssemblyParts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Argo
{
    public class NetListenerService : IHostedService
    {
        private List<INetListener> _netListeners;
        private NetServerOptions _options;
        private INetListenerProvider _netListenerFactory;
        private ILogger _logger;

        public NetListenerService(IOptions<NetServerOptions> options,
            INetListenerProvider netListenerFactory,
            ILoggerFactory loggerFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
            _netListeners = new List<INetListener>();
            _netListenerFactory = netListenerFactory ?? throw new ArgumentNullException(nameof(netListenerFactory));

            _logger = loggerFactory.CreateLogger<NetListenerService>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var opt in _options.ListenerOptions)
            {
                var netListener = _netListenerFactory.CreateListener(opt);
                _netListeners.Add(netListener);
                _logger.LogInformation($"NetListener mode:{opt.SocketMode} port:{opt.Port} starting");

                await netListener?.StartListenerAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var netListener in _netListeners)
            {
                _logger.LogInformation($"NetListener stopping");

                await netListener?.CloseListenerAsync();
            }
        }
    }
}