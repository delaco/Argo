using AdvancedServer.Test;
using Argo;
using Argo.Commands;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;

namespace AdvancedServer.Commands
{
    [Command(1)]
    public class UserEntryCommand : ResponseCommand<Proxy_UserEntryResp>
    {
        public UserEntryCommand(ILogger<UserEntryCommand> logger)
        {
            this.Logger = logger;
        }

        public ILogger<UserEntryCommand> Logger { get; set; }

        public override void ExecuteInternal(Proxy_UserEntryResp request)
        {
            Logger.LogInformation($"The request:{request.UserId} already processed");
        }
    }
}
