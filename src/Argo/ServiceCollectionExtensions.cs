using Argo.AssemblyParts;
using Argo.Commands;
using Argo.Internal;
using Argo.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Argo
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services,
            AssemblyPartManager assemblyPartManager,
            ListenerOptions netListenerOptions)
        {
            var commandAssemblies = new List<Assembly>();
            if (netListenerOptions.CommandAssemblies != null && netListenerOptions.CommandAssemblies.Any())
            {
                var definedAssemblies = AssemblyUtil.GetAssembliesFromStrings(netListenerOptions.CommandAssemblies.ToArray());

                if (definedAssemblies.Any())
                    commandAssemblies.AddRange(definedAssemblies);
            }

            if (!commandAssemblies.Any())
            {
                commandAssemblies.Add(Assembly.GetEntryAssembly());
            }

            foreach (var assembly in commandAssemblies)
            {
                assemblyPartManager.AssemblyParts.Add(new AssemblyPart(assembly));
            }

            var feature = new CommandFeature();
            assemblyPartManager.PopulateFeature(feature);
            foreach (var command in feature.Commands.Select(c => c.AsType()))
            {
                services.TryAddTransient(command, command);
            }

            services.TryAddSingleton<ICommandDescriptorCollectionProvider, CommandDescriptorCollectionProvider>();
            services.TryAddSingleton<ICommandDescriptorContainer, CommandDescriptorContainer>();
            services.TryAddTransient<ICommandActivator, DefaultCommandActivator>();

            return services;
        }

        public static IServiceCollection AddSocketClient(this IServiceCollection serviceCollection, IConfiguration config)
        {
            serviceCollection.Configure<RemoteOptions>(config);
            serviceCollection.AddSingleton<ClientWaits>();
            serviceCollection.AddSingleton<IMessageHandlerProvider, DotNettyMessageHandlerProvider>();
            serviceCollection.AddSingleton<ISocketClientProvider, DottNettyClientAdapter>();
            serviceCollection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceCollection.AddSingleton<ISocketClientPoolContainer, SocketClientPoolContainer>();
            return serviceCollection;
        }

        internal static T GetRequestService<T>(this IServiceCollection services)
        {
            return (T)services
                .LastOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }
    }
}
