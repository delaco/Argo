namespace Argo.Commands
{
    public interface ICommandDescriptorContainer
    {
        CommandDescriptor Get(RequestContext context);
    }
}