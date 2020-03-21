using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Argo
{
    internal class HeaderBasedFrameDecoder : LengthFieldBasedFrameDecoder
    {
        private readonly ByteOrder _byteOrder;

        public HeaderBasedFrameDecoder()
            : base(ByteOrder.LittleEndian, ushort.MaxValue, 4, 4, 4, 0, true)
        {
            _byteOrder = ByteOrder.LittleEndian;
        }

        protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            if (base.Decode(context, input) is IByteBuffer byteBuffer)
            {
                var cmdId = base.GetUnadjustedFrameLength(byteBuffer, 0, 4, _byteOrder);
                var seqId = base.GetUnadjustedFrameLength(byteBuffer, 8, 4, _byteOrder);

                var bodyBuff = byteBuffer.SkipBytes(12);
                var body = new byte[bodyBuff.ReadableBytes];
                bodyBuff.ReadBytes(body);
                var message = new MessagePacket((uint)cmdId, (uint)seqId, body);

                return message;
            }

            return input;
        }
    }
}
