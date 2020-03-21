using System;
using System.Collections.Generic;
using System.Reflection;

namespace Argo.AssemblyParts
{
    public class AssemblyPart
    {
        /// <summary>
        /// Initalizes a new <see cref="AssemblyPart"/> instance.
        /// </summary>
        /// <param name="assembly"></param>
        public AssemblyPart(Assembly assembly)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        /// <summary>
        /// Gets the <see cref="Assembly"/> of the <see cref="ApplicationPart"/>.
        /// </summary>
        public Assembly Assembly { get; }

        public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes;
    }
}
