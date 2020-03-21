using System.Collections.Generic;
using System.Reflection;

namespace Argo.Commands
{
    /// <summary>
    /// The list of commands types
    /// </summary>
    public class CommandFeature
    {
        /// <summary>
        /// Gets the list of command types.
        /// </summary>
        public IList<TypeInfo> Commands { get; } = new List<TypeInfo>();
    }
}
