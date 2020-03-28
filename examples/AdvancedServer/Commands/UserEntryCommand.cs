using AdvancedServer.Test;
using Argo;
using Argo.Commands;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace AdvancedServer.Commands
{
    [Command(1)]
    public class UserEntryCommand : ICommand
    {
        public UserEntryCommand(ILogger<UserEntryCommand> logger)
        {
            this.Logger = logger;
        }

        public ILogger<UserEntryCommand> Logger { get; set; }

        public void Execute(RequestContext requestContext)
        {
            var ret = ExecuteInternal(requestContext.Request);
            requestContext.AppSession?.SendAsync(ret);
        }

        internal IPacket ExecuteInternal(IPacket packet)
        {
            var req = Proxy_UserEntryReq.Parser.ParseFrom(packet.Body.ToArray());
            Logger.LogInformation($"The request:{req.UserId}) already processed");
            Proxy_UserEntryResp resp = new Proxy_UserEntryResp
            {
                UserId = req.UserId,
                Result = new ResponseResult() { Code = 111, Message = "test message" }
            };

            return new PacketInfo(packet.Command, packet.Sequence, resp.ToByteArray());
        }
    }
}
