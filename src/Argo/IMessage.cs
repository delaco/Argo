namespace Argo
{
    public interface IMessage
    {
        int Command { get; }

        int Sequence { get; }

        short Option { get; }

        byte[] Body { get; }

        byte[] Content { get; }
    }
}
