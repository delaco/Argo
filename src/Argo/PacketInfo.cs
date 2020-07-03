using System;

namespace Argo
{
    public class PacketInfo : IPacket
    {
        public int Command { get; }

        public int Sequence { get; }

        public byte[] Body { get; }

        public PacketInfo(int command, int sequence, byte[] body)
        {
            Command = command;
            Sequence = sequence;
            Body = body;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[Command={Command},Sequence={Sequence},Lenth={Body.Length}]";
        }
    }
}