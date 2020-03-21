using System;

namespace Argo.Commands
{
    public class CommandContext
    {
        public CommandContext(RequestContext requestContext,
            CommandDescriptor commandDescriptor,
            IServiceProvider requestServices)
        {
            CommandDescriptor = commandDescriptor ?? throw new ArgumentNullException(nameof(commandDescriptor));
            RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
            RequestServices = requestServices ?? throw new ArgumentNullException(nameof(requestServices));
        }

        public CommandDescriptor CommandDescriptor
        {
            get; set;
        }

        public RequestContext RequestContext
        {
            get; set;
        }

        public IServiceProvider RequestServices
        {
            get; set;
        }
    }
}