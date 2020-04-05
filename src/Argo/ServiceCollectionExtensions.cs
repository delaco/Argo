using Argo.AssemblyParts;
using Argo.Commands;
using Argo.Internal;
using Argo.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Argo
{
    public static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddCommands(this IServiceCollection services,
            AssemblyPartManager assemblyPartManager,
            List<string> commandAssemblyArry)
        {
            var commandAssemblies = new List<Assembly>();
            if (commandAssemblyArry != null && commandAssemblyArry.Any())
            {
                var definedAssemblies = AssemblyUtil.GetAssembliesFromStrings(commandAssemblyArry.ToArray());

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
            serviceCollection.AddSingleton<ITypeActivatorCache, TypeActivatorCache>();
            serviceCollection.AddSingleton<ClientMessageRouter, DefaultClientMessageRouter>();
            serviceCollection.AddSingleton<IPacketCodec, DefaultPacketCodec>();
            serviceCollection.AddSingleton<IMessageHandlerProvider, DotNettyMessageHandlerProvider>();
            serviceCollection.AddSingleton<ISocketClientProvider, DottNettyClientAdapter>();
            serviceCollection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceCollection.AddSingleton<ISocketClientPoolContainer, SocketClientPoolContainer>();
            var partManager = new AssemblyPartManager();
            serviceCollection.AddSingleton(partManager);
            if (!partManager.FeatureProviders.OfType<CommandFeatureProvider>().Any())
            {
                partManager.FeatureProviders.Add(new CommandFeatureProvider());
            }

            var options = ConfigurationBinder.Get<RemoteOptions>(config);
            serviceCollection.AddCommands(partManager, new List<string>() { options.CommandAssembly });

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
