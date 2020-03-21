namespace Argo
{
    public interface IMessage
    {
        uint CommandId { get; }

        uint SequenceId { get; }

        byte[] Body { get; }

        byte[] Content { get; }
    }
}
