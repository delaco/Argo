using Argo.Utils;
using System;

namespace Argo.Internal
{
    public class DefaultPacketCodec : IPacketCodec
    {
        public int CommandFieldOffset { get; }

        public int CommandFieldLength { get; }

        public int SequenceFieldOffset { get; }

        public int SequenceFieldLength { get; }

        public int LengthFieldOffset { get; }

        public int LengthFieldLength { get; }

        public int HeaderLenght { get; }

        public DefaultPacketCodec(int commandFieldOffset = 0,
            int commandFieldLength = 4,
            int sequenceFieldOffset = 6,
            int sequenceFieldLength = 4,
            int lengthFieldOffset = 10,
            int lengthFieldLength = 4,
            int headerLenght = 14)
        {
            CommandFieldOffset = commandFieldOffset;
            CommandFieldLength = commandFieldLength;
            SequenceFieldOffset = sequenceFieldOffset;
            SequenceFieldLength = sequenceFieldLength;
            LengthFieldOffset = lengthFieldOffset;
            LengthFieldLength = lengthFieldLength;
            HeaderLenght = headerLenght;
        }

        public virtual IPacket Decode(Span<byte> byteBuffer)
        {
            var command = ConvertUtil.GetFrameValue(byteBuffer, CommandFieldOffset, CommandFieldLength);
            var sequence = ConvertUtil.GetFrameValue(byteBuffer, SequenceFieldOffset, SequenceFieldLength);
            var bodyLength = ConvertUtil.GetFrameValue(byteBuffer, LengthFieldOffset, LengthFieldLength);
            var bodyBuffer = byteBuffer.Slice(HeaderLenght, (int)bodyLength);

            return new PacketInfo((int)command, (int)sequence, bodyBuffer.ToArray());
        }

        public virtual Span<byte> Encode(IPacket packet)
        {
            var bodyLength = packet.Body.Length;
            var commandBuffer = ConvertUtil.GetFrameBuffer(packet.Command, CommandFieldLength);
            var sequenceBuffer = ConvertUtil.GetFrameBuffer(packet.Sequence, SequenceFieldLength);
            var lengthBuffer = ConvertUtil.GetFrameBuffer(bodyLength, LengthFieldLength);

            Span<byte> resultBuffer = new byte[HeaderLenght + bodyLength];
            var resultCommand = resultBuffer.Slice(CommandFieldOffset, CommandFieldLength);
            commandBuffer.CopyTo(resultCommand);
            var resultSequence = resultBuffer.Slice(SequenceFieldOffset, SequenceFieldLength);
            sequenceBuffer.CopyTo(resultSequence);
            var resultLength = resultBuffer.Slice(LengthFieldOffset, LengthFieldLength);
            lengthBuffer.CopyTo(resultLength);

            var resultBody = resultBuffer.Slice(HeaderLenght, bodyLength);
            packet.Body.CopyTo(resultBody);

            return resultBuffer;
        }
    }
}
