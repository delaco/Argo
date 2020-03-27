using System;

namespace Argo
{
    public class PacketInfo : IPacket
    {
        private byte[] _body;
        public int Command { get; }

        public int Sequence { get; }

        public byte[] Body => _body;

        public PacketInfo(int command, int sequence, byte[] body)
        {
            Command = command;
            Sequence = sequence;
            _body = body;
        }

        public override string ToString()
        {
            return $"{GetType().Name}[Command={Command},Sequence={Sequence},Lenth={Body.Length}]";
        }
    }
}