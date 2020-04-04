using System;

namespace Argo.Commands
{
    public class CommandContext
    {
        public CommandContext(CommandDescriptor commandDescriptor,
            IServiceProvider requestServices)
        {
            CommandDescriptor = commandDescriptor ?? throw new ArgumentNullException(nameof(commandDescriptor));
            ServiceProvider = requestServices ?? throw new ArgumentNullException(nameof(requestServices));
        }

        public CommandDescriptor CommandDescriptor
        {
            get; set;
        }

        public IServiceProvider ServiceProvider
        {
            get; set;
        }
    }
}