using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Argo.Internal
{
    internal class DotNettyMessageEncoder : MessageToMessageEncoder<IPacket>
    {
        private IPacketCodec _packetCodec;

        public DotNettyMessageEncoder(IPacketCodec packetCodec)
        {
            _packetCodec = packetCodec;
        }

        protected override void Encode(IChannelHandlerContext context, IPacket message, List<object> output)
        {
            var bytes = _packetCodec.Encode(message);

            IByteBuffer byteBuffer = Unpooled.CopiedBuffer(bytes.ToArray());
            output.Add(byteBuffer.Retain());
        }
    }

    internal class DotNettyMessageWebSocketEncoder : DotNettyMessageEncoder
    {
        public DotNettyMessageWebSocketEncoder(IPacketCodec packetCodec) : base(packetCodec)
        {
        }

        public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            Task result;
            ThreadLocalObjectList output = null;
            try
            {
                if (base.AcceptOutboundMessage(msg))
                {
                    output = ThreadLocalObjectList.NewInstance();
                    var cast = (PacketInfo)msg;
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
                throw new EncoderException(e);
            }
            catch (Exception ex)
            {
                throw new EncoderException(ex);
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