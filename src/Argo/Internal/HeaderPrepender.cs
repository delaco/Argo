using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Argo.Net
{
    internal class HeaderPrepender : MessageToMessageEncoder<MessagePacket>
    {
        private readonly ByteOrder _byteOrder;
        private readonly int _lengthFieldLength;

        public HeaderPrepender()
        {
            _byteOrder = ByteOrder.LittleEndian;
            _lengthFieldLength = 4;
        }

        protected override void Encode(IChannelHandlerContext context, MessagePacket message, List<object> output)
        {
            int length = message.Body.Length;

            if (length < 0)
            {
                throw new ArgumentException($"Adjusted frame length ({ length }) is less than zero");
            }

            output.Add(context.Allocator.Buffer(4).WriteIntLE(message.Command));
            output.Add(context.Allocator.Buffer(2).WriteShortLE(message.Option));
            output.Add(context.Allocator.Buffer(4).WriteIntLE(message.Sequence));

            switch (this._lengthFieldLength)
            {
                case 1:
                    if (length > byte.MaxValue)
                    {
                        throw new ArgumentException("length of object does not fit into one byte: " + length);
                    }

                    output.Add(context.Allocator.Buffer(1).WriteByte((byte)length));
                    break;
                case 2:
                    if (length > ushort.MaxValue)
                    {
                        throw new ArgumentException("length of object does not fit into a short integer: " + length);
                    }

                    output.Add(context.Allocator.Buffer(2).WriteShortLE((ushort)length));
                    break;
                case 4:
                    output.Add(context.Allocator.Buffer(4).WriteIntLE(length));
                    break;
                default:
                    throw new Exception("Unknown length field length");
            }


            IByteBuffer byteBuffer = Unpooled.CopiedBuffer(message.Body);
            output.Add(byteBuffer.Retain());
        }
    }

    internal class WebSocketHeaderPrepender : HeaderPrepender
    {
        public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            Task result;
            ThreadLocalObjectList output = null;
            try
            {
                if (base.AcceptOutboundMessage(msg))
                {
                    output = ThreadLocalObjectList.NewInstance();
                    var cast = (MessagePacket)msg;
                    try
                    {
                        base.Encode(ctx, cast, output);
                    }
                    finally
                    {
                        ReferenceCountUtil.Release(cast);
                    }

                    if (output.Count == 0)
                    {
                        output.Return();
                        output = null;

                        throw new EncoderException(this.GetType().Name + " must produce at least one message.");
                    }
                }
                else
                {
                    return ctx.WriteAsync(msg);
                }
            }
            catch (EncoderException e)
            {
                return TaskEx.FromException(e);
            }
            catch (Exception ex)
            {
                return TaskEx.FromException(new EncoderException(ex));
            }
            finally
            {
                if (output != null)
                {
                    if (output.Count > 0)
                    {
                        IByteBuffer byteBuffer = Unpooled.CopiedBuffer(output.ConvertAll(v => v as IByteBuffer).ToArray());
                        result = ctx.WriteAsync(new BinaryWebSocketFrame(byteBuffer));
                    }
                    else
                    {
                        // 0 items in output - must never get here
                        result = null;
                    }
                    output.Return();
                }
                else
                {
                    // output was reset during exception handling - must never get here
                    result = null;
                }
            }

            return result;
        }
    }
}