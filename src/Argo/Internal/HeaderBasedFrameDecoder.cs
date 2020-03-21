using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Argo
{
    internal class HeaderBasedFrameDecoder : LengthFieldBasedFrameDecoder
    {
        private readonly ByteOrder _byteOrder;

        public HeaderBasedFrameDecoder()
            : base(ByteOrder.LittleEndian, ushort.MaxValue, 10, 4, 0, 0, true)
        {
            _byteOrder = ByteOrder.LittleEndian;
        }

        protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            if (base.Decode(context, input) is IByteBuffer byteBuffer)
            {
                var cmd = base.GetUnadjustedFrameLength(byteBuffer, 0, 4, _byteOrder);
                var opt = base.GetUnadjustedFrameLength(byteBuffer, 4, 2, _byteOrder);
                var seq = base.GetUnadjustedFrameLength(byteBuffer, 6, 4, _byteOrder);

                var bodyBuff = byteBuffer.SkipBytes(14);
                var body = new byte[bodyBuff.ReadableBytes];
                bodyBuff.ReadBytes(body);

                var message = new MessagePacket((int)cmd, (short)opt, (int)seq, body);

                return message;
            }

            return input;
        }
    }
}
