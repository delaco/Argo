namespace Argo.Commands
{
    public interface ICommandDescriptorCollectionProvider
    {
        /// <summary>
        /// Returns the current cached <see cref="CommandDescriptor"/>
        /// </summary>
        CommandDescriptorCollection CommandDescriptors { get; }
    }
}