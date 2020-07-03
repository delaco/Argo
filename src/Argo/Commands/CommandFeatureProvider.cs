using Argo.AssemblyParts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace Argo.Commands
{
    /// <summary>
    /// Discovers commands from a list of <see cref="AssemblyPart"/> instances.
    /// </summary>
    public class CommandFeatureProvider : IFeatureProvider<CommandFeature>
    {
        private const string CommandTypeNameSuffix = "Command";

        public void PopulateFeature(
            IEnumerable<AssemblyPart> parts,
            CommandFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            foreach (var part in parts.OfType<AssemblyPart>())
            {
                foreach (var type in part.Types)
                {
                    if (IsCommand(type) && !feature.Commands.Contains(type))
                    {
                        feature.Commands.Add(type);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a given <paramref name="typeInfo"/> is a command.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/> candidate.</param>
        /// <returns><code>true</code> if the type is a command; otherwise <code>false</code>.</returns>
        protected virtual bool IsCommand(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            if (!typeInfo.IsPublic)
            {
                return false;
            }

            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }

            if (typeInfo.IsDefined(typeof(NonCommandAttribute)))
            {
                return false;
            }

            if (!typeInfo.Name.EndsWith(CommandTypeNameSuffix, StringComparison.OrdinalIgnoreCase) &&
              !typeInfo.IsDefined(typeof(CommandAttribute)))
            {
                return false;
            }

            return true;
        }
    }
}