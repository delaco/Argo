using AdvancedServer.Test;
using Argo;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                  .AddJsonFile("appsettings.json")
                  .Build();

            var host = new HostBuilder()
                .ConfigureServices((service) =>
                {
                    service.AddSocketClient(config);
                    service.AddHostedService<ClientService>();
                })
                .ConfigureLogging((loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("logging"));
                    loggingBuilder.AddConsole();
                })
                .Build();

            host.StartAsync().GetAwaiter().GetResult();
        }
    }

    class ClientService : IHostedService
    {
        ISocketClientPoolContainer socketClientPoolContainer;
        ILogger<ClientService> logger;

        public ClientService(ISocketClientPoolContainer socketClientPoolContainer, ILogger<ClientService> logger)
        {
            this.socketClientPoolContainer = socketClientPoolContainer;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var client = socketClientPoolContainer.Get("localhost");
                var random = new Random(1);
                for (var i = 0; i < 10; i++)
                {
                    Proxy_UserEntryReq req = new Proxy_UserEntryReq()
                    {
                        UserId = random.Next(1, short.MaxValue),
                        UserName = "userTest"
                    };
                    var packet = new PacketInfo(1, 1, req.ToByteArray());
                    client.SendAsync(packet).GetAwaiter().GetResult();
                    logger.LogInformation($"The {req.UserId} send request");
                }
                socketClientPoolContainer.Return(client);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
