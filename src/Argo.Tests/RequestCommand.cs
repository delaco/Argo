using Argo.Commands;
using DotNetty.Codecs;
using Google.Protobuf;

namespace Argo.Tests
{
    public abstract class RequestCommand<TRequest, TResponse> : ICommand
        where TRequest : IMessage<TRequest>, new()
        where TResponse : IMessage<TResponse>, new()
    {
        public void Execute(RequestContext requestContext)
        {
            CodedInputStream codedInputStream = new CodedInputStream(requestContext.Request.Body.ToArray());

            MessageParser<TRequest> messageParser = new MessageParser<TRequest>(() => new TRequest());
            var request = messageParser.ParseFrom(codedInputStream);
            if (request != null)
            {
                var response = ExecuteInternal(request);
                if (response != null)
                {
                    var responsePacket = new PacketInfo(requestContext.Request.Command, requestContext.Request.Sequence, response.ToByteArray());
                    requestContext.AppSession.SendAsync(responsePacket).ConfigureAwait(false);
                }
            }
            else
            {
                throw new CodecException();
            }
        }

        public abstract TResponse ExecuteInternal(TRequest request);
    }
}
