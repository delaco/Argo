namespace Argo
{
    /// <summary>
    /// Provides methods to create an <see cref="INetListener">.
    /// </summary>
    public interface INetListenerProvider
    {
        INetListener CreateListener(NetListenerOptions options);
    }
}
