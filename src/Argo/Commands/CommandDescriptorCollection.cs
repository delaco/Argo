using System;
using System.Collections.Generic;

namespace Argo.Commands
{
    /// <summary>
    /// A cached collection of <see cref="CommandDescriptor" />.
    /// </summary>
    public class CommandDescriptorCollection
    {
        public CommandDescriptorCollection(IReadOnlyList<CommandDescriptor> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Returns the cached <see cref="IReadOnlyList{ActionDescriptor}"/>.
        /// </summary>
        public IReadOnlyList<CommandDescriptor> Items { get; }
    }
}