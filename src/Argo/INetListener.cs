using System.Threading.Tasks;

namespace Argo
{
    public interface INetListener
    {
        Task StartAsync();

        Task CloseAsync();
    }
}
