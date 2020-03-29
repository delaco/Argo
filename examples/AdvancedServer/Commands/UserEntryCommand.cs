using AdvancedServer.Test;
using Argo;
using Argo.Commands;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;

namespace AdvancedServer.Commands
{
    [Command(1)]
    public class UserEntryCommand : RequestCommand<Proxy_UserEntryReq, Proxy_UserEntryResp>
    {
        public UserEntryCommand(ILogger<UserEntryCommand> logger)
        {
            this.Logger = logger;
        }

        public ILogger<UserEntryCommand> Logger { get; set; }

        public override Proxy_UserEntryResp ExecuteInternal(Proxy_UserEntryReq request)
        {
            Logger.LogInformation($"The request:{request.UserId}) already processed");
            return new Proxy_UserEntryResp
            {
                UserId = request.UserId,
                Result = new ResponseResult() { Code = 111, Message = "test message" }
            };
        }
    }
}
