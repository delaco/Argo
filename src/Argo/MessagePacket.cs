namespace Argo
{
    public class MessagePacket : IMessage
    {
        public byte[] Content { get; }

        public byte[] Body { get; }

        public int Command { get; }

        public int Sequence { get; }

        public short Option { get; } = 0;

        public MessagePacket(int command, short option, int sequence, byte[] body)
        {
            Command = command;
            Option = option;
            Sequence = sequence;
            Body = body;
        }

        public MessagePacket(int command, int sequence, byte[] body)
        {
            Command = command;
            Sequence = sequence;
            Body = body;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}[Command={this.Command},Sequence={this.Sequence},Lenth={Body.Length}]";
        }
    }
}