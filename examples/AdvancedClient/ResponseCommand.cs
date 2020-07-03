using Argo;
using Argo.Commands;
using DotNetty.Buffers;
using DotNetty.Codecs;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdvancedServer
{
    public abstract class ResponseCommand<TResponse> : ICommand
        where TResponse : IMessage<TResponse>, new()
    {
        public void Execute(RequestContext requestContext)
        {
            CodedInputStream codedInputStream = new CodedInputStream(requestContext.Request.Body);

            MessageParser<TResponse> messageParser = new MessageParser<TResponse>(() => new TResponse());
            var response = messageParser.ParseFrom(codedInputStream);
            if (response != null)
            {
                ExecuteInternal(response);
            }
            else
            {
                throw new CodecException();
            }
        }

        public abstract void ExecuteInternal(TResponse request);
    }
}
