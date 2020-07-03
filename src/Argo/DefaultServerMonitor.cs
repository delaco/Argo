using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Argo
{
    public class DefaultServerMonitor : IMonitor
    {
        ILogger<DefaultServerMonitor> Logger { get; }
        ServerOptions ServerOptions { get; }

        AppSessionContainer<AppSession> AppSessionContainer { get; }
        public DefaultServerMonitor(IOptions<ServerOptions> options,
            AppSessionContainer<AppSession> appSessionContainer,
            ILogger<DefaultServerMonitor> logger)
        {
            ServerOptions = options.Value;
            AppSessionContainer = appSessionContainer;
            Logger = logger;
        }

        public virtual async Task Execute()
        {
            await ClearAppSession();
        }

        private async Task ClearAppSession()
        {
            var now = DateTime.Now;
            foreach (var appSession in AppSessionContainer.Members.Values)
            {
                var timeSpan = now - appSession.LastAccessTime;
                if (timeSpan.TotalSeconds > ServerOptions.SessionDeadTime)
                {
                    try
                    {
                        await appSession.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning($"The {appSession} catch an exception:{ex} when close");
                    }

                    Logger.LogInformation($"The session:{appSession} has been cleaned");
                }
                else
                {
                    await Task.CompletedTask;
                }
            }
        }
    }
}
