using System;

namespace Argo
{
    public interface IMessage
    {
        int Command { get; }

        int Sequence { get; }

        Span<byte> Body { get; }
    }
}
