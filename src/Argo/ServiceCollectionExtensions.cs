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
        internal static IServiceCollection AddCommands(this IServiceCollection services)
        {
            var assemblyPartManager = new AssemblyPartManager();
            services.AddSingleton(assemblyPartManager);
            if (!assemblyPartManager.FeatureProviders.OfType<CommandFeatureProvider>().Any())
            {
                assemblyPartManager.FeatureProviders.Add(new CommandFeatureProvider());
            }

            services.AddSingleton<ITypeActivatorCache, TypeActivatorCache>();
            services.TryAddSingleton<ICommandDescriptorCollectionProvider, CommandDescriptorCollectionProvider>();
            services.TryAddSingleton<ICommandDescriptorContainer, CommandDescriptorContainer>();
            services.TryAddTransient<ICommandActivator, DefaultCommandActivator>();

            var commandAssemblies = new List<Assembly>();

            if (!commandAssemblies.Any())
            {
                commandAssemblies.Add(Assembly.GetEntryAssembly());
            }

            foreach (var assembly in commandAssemblies)
            {
                assemblyPartManager.AssemblyParts.Add(new AssemblyPart(assembly));
            }

            return services;
        }

        public static IServiceCollection AddSocketClient(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RemoteOptions>(config);
            services.AddSingleton<ClientWaits>();
            services.AddSingleton<ClientMessageRouter, DefaultClientMessageRouter>();
            services.AddSingleton<IPacketCodec, DefaultPacketCodec>();
            services.AddSingleton<IMessageHandlerProvider, DotNettyMessageHandlerProvider>();
            services.AddSingleton<ISocketClientProvider, DottNettyClientAdapter>();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<ISocketClientPoolContainer, SocketClientPoolContainer>();
            services.AddCommands();

            return services;
        }

        internal static T GetRequestService<T>(this IServiceCollection services)
        {
            return (T)services
                .LastOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }
    }
}
