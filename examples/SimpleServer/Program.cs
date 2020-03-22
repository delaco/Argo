using Argo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            var host = new HostBuilder()
                .UseArgo(config)
                .ConfigureServices((service) =>
                {
                    service.AddSingleton<IMessageCodec, CustomMessageCodec>();
                    service.AddSocketClient(config);
                })
                .ConfigureLogging((loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(config.GetSection("logging"));
                    loggingBuilder.AddConsole();
                })
                .Build();

            await host.StartAsync();
        }
    }
}
