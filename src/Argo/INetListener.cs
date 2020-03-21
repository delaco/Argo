using System.Threading.Tasks;

namespace Argo
{
    public interface INetListener
    {
        Task StartListenerAsync();

        Task CloseListenerAsync();
    }
}
