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
            var ret = this.ExecuteCore(requestContext.Packet);
            requestContext.Session?.SendAsync(ret);
        }

        internal PacketInfo ExecuteCore(IPacket packet)
        {
            ///var req = Proxy_UserEntryReq.Parser.ParseFrom(packet.Body.ToArray());
            ///Logger.LogInformation($"The request:{req.UserId}) already processed");A

            Proxy_UserEntryResp resp = new Proxy_UserEntryResp();
            resp.UserId = 1;
            resp.Result = new ResponseResult() { Code = 111, Message = "test message" };

            return new PacketInfo(packet.Command, packet.Sequence, resp.ToByteArray());
        }
    }
}
