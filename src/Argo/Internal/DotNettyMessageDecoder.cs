using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Argo.Internal
{
    internal class DotNettyMessageDecoder : LengthFieldBasedFrameDecoder
    {
        private IPacketCodec _packetCodec;

        public DotNettyMessageDecoder(IPacketCodec packetCodec)
            : base(ByteOrder.LittleEndian,
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
                var bodyLength = base.GetUnadjustedFrameLength(input, _packetCodec.LengthFieldOffset, _packetCodec.LengthFieldLength, ByteOrder.LittleEndian);
                var bytes = new byte[_packetCodec.HeaderLenght + bodyLength];

                byteBuffer.ReadBytes(bytes);

                var message = _packetCodec.Decode(bytes);

                return message;
            }

            return input;
        }
    }
}
