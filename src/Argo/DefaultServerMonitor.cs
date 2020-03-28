using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Argo
{
    public class DefaultServerMonitor : IServerMonitor
    {
        public virtual Task Execute() {
            return Task.CompletedTask;
        }
    }
}
