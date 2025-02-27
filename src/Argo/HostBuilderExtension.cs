﻿using Argo.AssemblyParts;
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
                services.Configure<ServerOptions>(config);
                services.AddSingleton<IPassThroughRuleProvider, PassThroughRuleProvider>();
                services.AddSingleton<IMessageRouter, MessageRouter>();
                services.AddSingleton<IPacketCodec, DefaultPacketCodec>();
                services.AddSingleton<INetListenerProvider, DotNettyListenerProvider>();
                var appSessionContainer = new AppSessionContainer<AppSession>();
                services.AddSingleton(appSessionContainer);
                services.AddSingleton<IHostedService, NetListenerService>();
                services.AddSingleton<IServerMonitor, DefaultServerMonitor>();
                services.AddSingleton<IHostedService, MonitorService>();
                var options = ConfigurationBinder.Get<ServerOptions>(config);
                if (options != null && options.Listeners.Any())
                {
                    options.Listeners.ForEach(op =>
                    {
                        services.AddCommands(op.CommandAssemblies);
                    });
                }
            });
        }
    }
}