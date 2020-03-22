using Argo;
using Argo.Commands;
using Microsoft.Extensions.Logging;

namespace SimpleServer.Commands
{
    [Command(1)]
    public class TestCommond : ICommand
    {
        private ILogger<TestCommond> _logger;

        public TestCommond(ILogger<TestCommond> logger)
        {
            this.Logger = logger;
        }

        public ILogger<TestCommond> Logger { get => _logger; set => _logger = value; }

        public void Execute(RequestContext requestContext)
        {
            var ret = this.ExecuteCore(requestContext.Message);
            requestContext.Session?.SendAsync(ret);
        }

        internal MessagePacket ExecuteCore(IMessage message)
        {
            return new MessagePacket(123, message.Sequence, new byte[] { 1, 2, 3, 4 });
        }
    }
}
