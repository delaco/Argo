using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Argo.Internal
{
    internal class DotNettyMessageDecoder : LengthFieldBasedFrameDecoder
    {
        private IMessageCodec _messageCodec;

        public DotNettyMessageDecoder(IMessageCodec messageCodec)
            : base(ByteOrder.LittleEndian,
                  ushort.MaxValue,
                  messageCodec.LengthFieldOffset,
                  messageCodec.LengthFieldLength,
                  messageCodec.HeaderLenght - messageCodec.LengthFieldOffset - messageCodec.LengthFieldLength,
                  0,
                  true)
        {
            _messageCodec = messageCodec;
        }

        protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
        {
            if (base.Decode(context, input) is IByteBuffer byteBuffer)
            {
                var bytes = new byte[input.ReadableBytes];
                input.ReadBytes(bytes);

                var message = _messageCodec.Decode(bytes);

                return message;
            }

            return input;
        }
    }
}
