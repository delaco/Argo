using Argo.AssemblyParts;
using Argo.Commands;
using Argo.Internal;
using Argo.PassThrough;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Argo
{
    public static class HostBuilderExtension
    {
        public static IHostBuilder UseArgo(this IHostBuilder builder, IConfiguration config)
        {
            return builder.ConfigureServices((services) =>
            {
                var partManager = new AssemblyPartManager();
                services.AddSingleton(partManager);
                ConfigureFeatureProviders(partManager);
                services.AddSingleton<ITypeActivatorCache, TypeActivatorCache>();
                services.AddSingleton<IPassThroughRuleProvider, PassThroughRuleProvider>();
                services.AddSingleton<IMessageRouter, MessageRouter>();
                services.Configure<NetServerOptions>(config);
                services.AddSingleton<INetListenerProvider, NetListenerProvider>();
                var sessionContainer = new SessionContainer<Session>();
                services.AddSingleton(sessionContainer);
                services.AddSingleton<IHostedService, NetListenerService>();
                var options = ConfigurationBinder.Get<NetServerOptions>(config);
                if (options != null && options.ListenerOptions.Any())
                {
                    options.ListenerOptions.ForEach(op =>
                    {
                        services.AddCommands(partManager, op);
                    });
                }
            });
        }

        private static void ConfigureFeatureProviders(AssemblyPartManager manager)
        {
            if (!manager.FeatureProviders.OfType<CommandFeatureProvider>().Any())
            {
                manager.FeatureProviders.Add(new CommandFeatureProvider());
            }
        }
    }
}