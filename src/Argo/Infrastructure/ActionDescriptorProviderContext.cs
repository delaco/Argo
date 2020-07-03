using System.Collections.Generic;

namespace Argo.Infrastructure
{
    public class ActionDescriptorProviderContext
    {
        public IList<ActionDescriptor> Results { get; } = new List<ActionDescriptor>();
    }
}
