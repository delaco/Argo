using System;

namespace Argo
{
    public class MessagePacket : IMessage
    {
        private byte[] _body;
        public int Command { get; }

        public int Sequence { get; }

        public Span<byte> Body => _body;

        public MessagePacket(int command, int sequence, byte[] body)
        {
            Command = command;
            Sequence = sequence;
            _body = body;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}[Command={this.Command},Sequence={this.Sequence},Lenth={Body.Length}]";
        }
    }
}