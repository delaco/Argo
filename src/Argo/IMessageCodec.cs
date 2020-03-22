using System;
using System.Collections.Generic;
using System.Text;

namespace Argo
{
    public interface IMessageCodec
    {
        int CommandFieldOffset { get; }

        int CommandFieldLength { get; }

        int SequenceFieldOffset { get; }

        int SequenceFieldLength { get; }

        int LengthFieldOffset { get; }

        int LengthFieldLength { get; }

        int HeaderLenght { get; }

        IMessage Decode(Span<byte> byteBuffer);

        Span<byte> Encode(IMessage message);
    }
}
