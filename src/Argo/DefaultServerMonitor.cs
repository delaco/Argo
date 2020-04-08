using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Argo
{
    public class DefaultServerMonitor : IServerMonitor
    {
        ILogger<DefaultServerMonitor> Logger { get; }
        ServerOptions ServerOptions { get; }

        AppSessionContainer<AppSession> AppSessionContainer { get; }
        public DefaultServerMonitor(IOptions<ServerOptions> options,
            AppSessionContainer<AppSession> appSessionContainer,
            ILogger<DefaultServerMonitor> logger
            )
        {
            ServerOptions = options.Value;
            AppSessionContainer = appSessionContainer;
            Logger = logger;
        }

        public virtual Task Execute()
        {
            return Task.Run(() =>
            {
                ClearAppSession();
            });
        }

        private void ClearAppSession()
        {
            var now = DateTime.Now;
            foreach (var appSession in AppSessionContainer.Members.Values)
            {
                var timeSpan = now - appSession.LastAccessTime;
                if (timeSpan.TotalSeconds > ServerOptions.SessionDeadTime)
                {
                    try
                    {
                        appSession.CloseAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning($"The {appSession} catch an exception:{ex} when close");
                    }
                    Logger.LogInformation($"Thesession:{appSession} has been cleaned");
                }
            }
        }
    }
}
