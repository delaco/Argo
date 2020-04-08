using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Argo
{
    public class MonitorService : BackgroundService
    {
        private ILogger<MonitorService> _logger;
        private ServerOptions _options;
        private IServerMonitor _serverMonitor;

        protected IServiceProvider ServiceProvider { get; }

        public MonitorService(IServiceProvider serviceProvider,
            IOptions<ServerOptions> options,
            IServerMonitor serverMonitor,
            ILogger<MonitorService> logger)
        {
            ServiceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
            _serverMonitor = serverMonitor;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested
                && _options.InvalidSessionRelease
                && _serverMonitor != null)
            {
                await _serverMonitor.Execute().ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromSeconds(_options.MonitorInterval), stoppingToken);
            }
        }
    }
}
