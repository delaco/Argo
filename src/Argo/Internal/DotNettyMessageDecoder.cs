using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;

namespace Argo.Internal
{
    internal class DotNettyMessageDecoder : LengthFieldBasedFrameDecoder
    {
        private IPacketCodec _packetCodec;

        public DotNettyMessageDecoder(IPacketCodec packetCodec)
            : base(packetCodec.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian,
                  ushort.MaxValue,
                  packetCodec.LengthFieldOffset,
                  packetCodec.LengthFieldLength,
                  packetCodec.HeaderLenght - packetCodec.LengthFieldOffset - packetCodec.LengthFieldLength,
                  0,
                  true)
        {
            _packetCodec = packetCodec;
        }

        protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            if (base.Decode(context, input) is IByteBuffer byteBuffer)
            {
                //var bodyLength = base.GetUnadjustedFrameLength(input, _packetCodec.LengthFieldOffset, _packetCodec.LengthFieldLength, _packetCodec.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian);
                var readBytes = new byte[byteBuffer.ReadableBytes];

                byteBuffer.ReadBytes(readBytes);
                var message = _packetCodec.Decode(readBytes);

                return message;
            }

            return input;
        }
    }
}
