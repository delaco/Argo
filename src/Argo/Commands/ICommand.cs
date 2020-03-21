namespace Argo.Commands
{
    /// <summary>
    /// Represents a command handler
    /// </summary>
    public interface ICommand
    {
        void Execute(RequestContext requestContext);
    }
}