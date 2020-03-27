using System;

namespace Argo
{
    public interface IPacket
    {
        int Command { get; }

        int Sequence { get; }

        byte[] Body { get; }
    }
}
