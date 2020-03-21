namespace Argo
{
    public class MessagePacket : IMessage
    {
        public byte[] Content { get; }

        public byte[] Body { get; }

        public uint CommandId { get; }

        public uint SequenceId { get; }

        public MessagePacket(uint commandId, uint sequenceId, byte[] body)
        {
            CommandId = commandId;
            SequenceId = sequenceId;
            Body = body;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}[CommandId={this.CommandId},SequenceId={this.SequenceId},Lenth={Body.Length}]";
        }
    }
}