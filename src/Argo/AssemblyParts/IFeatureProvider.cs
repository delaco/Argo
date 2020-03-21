using System.Collections.Generic;

namespace Argo.AssemblyParts
{
    /// <summary>
    /// Marker interface for <see cref="IFeatureProvider"/>
    /// implementations.
    /// </summary>
    public interface IFeatureProvider
    {
    }

    /// <summary>
    /// A provider for a given <typeparamref name="TFeature"/> feature.
    /// </summary>
    /// <typeparam name="TFeature">The type of the feature.</typeparam>
    public interface IFeatureProvider<TFeature> : IFeatureProvider
    {
        /// <summary>
        /// Updates the <paramref name="feature"/> instance.
        /// </summary>
        /// <param name="parts">The list of <see cref="AssemblyPart"/>s of the
        /// host.
        /// </param>
        /// <param name="feature">The feature instance to populate.</param>
        void PopulateFeature(IEnumerable<AssemblyPart> parts, TFeature feature);
    }
}