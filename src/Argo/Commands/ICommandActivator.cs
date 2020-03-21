namespace Argo.Commands
{
    /// <summary>
    /// Provides methods to create a command.
    /// </summary>
    public interface ICommandActivator
    {
        /// <summary>
        /// Creates a command.
        /// </summary>
        /// <param name="context">The <see cref="CommandContext"/> for the executing request.</param>
        object Create(CommandContext commandContext);

        /// <summary>
        /// Releases a command.
        /// </summary>
        /// <param name="context">The <see cref="CommandContext"/> for the executing request.</param>
        /// <param name="controller">The command to release.</param>
        void Release(CommandContext commandContext, object command);
    }
}