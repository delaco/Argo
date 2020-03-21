using System;
using System.Collections.Generic;
using System.Linq;

namespace Argo.AssemblyParts
{
    /// <summary>
    /// Manages the parts and features of a host.
    /// </summary>
    public class AssemblyPartManager
    {
        /// <summary>
        /// Gets the list of <see cref="IFeatureProvider"/>s.
        /// </summary>
        public IList<IFeatureProvider> FeatureProviders { get; } =
            new List<IFeatureProvider>();

        /// <summary>
        /// Gets the list of <see cref="AssemblyPart"/>s.
        /// </summary>
        public IList<AssemblyPart> AssemblyParts { get; } =
            new List<AssemblyPart>();

        /// <summary>
        /// Populates the given <paramref name="feature"/> using the list of
        /// <see cref="IFeatureProvider{TFeature}"/>s configured on the
        /// <see cref="AssemblyPartManager"/>.
        /// </summary>
        /// <typeparam name="TFeature">The type of the feature.</typeparam>
        /// <param name="feature">The feature instance to populate.</param>
        public void PopulateFeature<TFeature>(TFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            foreach (var provider in FeatureProviders.OfType<IFeatureProvider<TFeature>>())
            {
                provider.PopulateFeature(AssemblyParts, feature);
            }
        }
    }
}