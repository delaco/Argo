using System;

namespace Argo.Internal
{
    public class DefaultMessageCodec : IMessageCodec
    {
        public int CommandFieldOffset { get; }

        public int CommandFieldLength { get; }

        public int SequenceFieldOffset { get; }

        public int SequenceFieldLength { get; }

        public int LengthFieldOffset { get; }

        public int LengthFieldLength { get; }

        public int HeaderLenght { get; }

        public DefaultMessageCodec(int commandFieldOffset = 0,
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

        public virtual IMessage Decode(Span<byte> byteBuffer)
        {
            var command = GetFrameValue(byteBuffer, CommandFieldOffset, CommandFieldLength);
            var sequence = GetFrameValue(byteBuffer, SequenceFieldOffset, SequenceFieldLength);
            var bodyLength = GetFrameValue(byteBuffer, LengthFieldOffset, LengthFieldLength);
            var bodyBuffer = byteBuffer.Slice(HeaderLenght, (int)bodyLength);

            return new MessagePacket((int)command, (int)sequence, bodyBuffer.ToArray());
        }

        public virtual Span<byte> Encode(IMessage message)
        {
            var bodyLength = message.Body.Length;
            var commandBuffer = GetFrameBuffer(message.Command, CommandFieldLength);
            var sequenceBuffer = GetFrameBuffer(message.Sequence, SequenceFieldLength);
            var lengthBuffer = GetFrameBuffer(bodyLength, LengthFieldLength);

            Span<byte> resultBuffer = new byte[HeaderLenght + bodyLength];
            var resultCommand = resultBuffer.Slice(CommandFieldOffset, CommandFieldLength);
            commandBuffer.CopyTo(resultCommand);
            var resultSequence = resultBuffer.Slice(SequenceFieldOffset, SequenceFieldLength);
            sequenceBuffer.CopyTo(resultSequence);
            var resultLength = resultBuffer.Slice(LengthFieldOffset, LengthFieldLength);
            lengthBuffer.CopyTo(resultLength);

            var resultBody = resultBuffer.Slice(HeaderLenght, bodyLength);
            message.Body.CopyTo(resultBody);

            return resultBuffer;
        }

        protected virtual long GetFrameValue(Span<byte> byteBuffer, int fieldOffset, int fieldlength)
        {
            var frameLength = fieldlength switch
            {
                1 => byteBuffer[0],
                2 => BitConverter.ToInt16(byteBuffer.Slice(fieldOffset, fieldlength)),
                4 => BitConverter.ToInt32(byteBuffer.Slice(fieldOffset, fieldlength)),
                _ => throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4)"),
            };

            return frameLength;
        }

        protected virtual Span<byte> GetFrameBuffer(int fieldValue, int fieldlength)
        {
            var biteBuffer = fieldlength switch
            {
                1 => new byte[] { (byte)fieldValue },
                2 => BitConverter.GetBytes((short)fieldValue),
                4 => BitConverter.GetBytes(fieldValue),
                _ => throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4)"),
            };

            return biteBuffer;
        }
    }
}
