namespace Argo.Infrastructure
{
    public interface IActionDescriptorCollectionProvider
    {
        /// <summary>
        /// Returns the current cached <see cref="ActionDescriptorCollection"/>
        /// </summary>
        ActionDescriptorCollection ActionDescriptors { get; }
    }
}
