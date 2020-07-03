using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Argo
{
    public interface IMonitor
    {
        Task Execute();
    }
}
