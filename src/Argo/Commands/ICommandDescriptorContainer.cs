namespace Argo.Commands
{
    public interface ICommandDescriptorContainer
    {
        CommandDescriptor Get(int command);
    }
}