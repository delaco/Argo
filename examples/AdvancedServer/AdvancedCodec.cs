using System;
using System.Collections.Generic;
using System.Text;
using Argo;

namespace AdvancedServer
{
    public class AdvancedCodec : DefaultPacketCodec
    {
        public override IPacket Decode(Span<byte> byteBuffer)
        {
            return base.Decode(byteBuffer);
        }

        public override Span<byte> Encode(IPacket packet)
        {
            return base.Encode(packet);
        }
    }

    public class PacketInfo<T> : PacketInfo
    {
        public T TypeData { get; set; }

        public PacketInfo(int command, int sequence, byte[] body) : base(command, sequence, body)
        {
        }
    }
}
