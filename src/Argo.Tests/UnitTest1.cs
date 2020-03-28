using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Argo.Commands;
using AdvancedServer.Test;
using Argo.Internal;
using Google.Protobuf;

namespace Argo.Tests
{
    [Command(1)]
    public class TestCommond : ICommand
    {
        public TestCommond()
        {

        }

        public void Execute(RequestContext requestContext)
        {
            requestContext.AppSession?.SendAsync(requestContext.Request);
        }
    }

    public class UnitTest1
    {
        public IServiceProvider Services { get; }

        public UnitTest1()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            var host = new HostBuilder()
                .UseArgo(config)
                .ConfigureServices((service) =>
                {
                    service.AddSocketClient(config);
                })
                .ConfigureLogging((loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("logging"));
                    loggingBuilder.AddConsole();
                })
                .Build();

            host.StartAsync().GetAwaiter().GetResult();
            Services = host.Services;
        }

        [Fact]
        public void SimpleRequestTest()
        {
            var poolContainer = Services.GetRequiredService<ISocketClientPoolContainer>();
            Assert.NotNull(poolContainer);
            var client = poolContainer.Get("localhost");
            Assert.NotNull(client);
            var body = new byte[] { 1, 0, 1, 0 };
            var packet = new PacketInfo(1, 0, body);
            var ret = client.Send(packet).GetAwaiter().GetResult();
            poolContainer.Return(client);
            Assert.NotNull(ret);
        }
    }
}
