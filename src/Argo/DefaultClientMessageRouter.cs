using Argo.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Argo
{
    public class DefaultClientMessageRouter : ClientMessageRouter
    {
        private IServiceProvider _serviceProvider;
        private ICommandDescriptorContainer _commandContainer;
        private ICommandActivator _commandActivator;
        private ILogger<DefaultClientMessageRouter> _logger;

        public DefaultClientMessageRouter(IServiceProvider serviceProvider,
            ICommandDescriptorContainer commandContainer,
            ICommandActivator commandActivator,
            ILogger<DefaultClientMessageRouter> logger)
        {
            _serviceProvider = serviceProvider ?? throw new NullReferenceException(nameof(serviceProvider));
            _commandContainer = commandContainer ?? throw new NullReferenceException(nameof(commandContainer));
            _commandActivator = commandActivator ?? throw new NullReferenceException(nameof(commandActivator));
            _logger = logger;
        }

        public override void Route(RequestContext requestContext)
        {
            Contract.Requires(requestContext != null);
            var commandDescriptor = this._commandContainer.Get(requestContext.Request.Command);
            if (commandDescriptor != null)
            {
                var commandContext = new CommandContext(commandDescriptor, _serviceProvider);
                if (!(_commandActivator.Create(commandContext) is ICommand command))
                {
                    throw new NotImplementedException(nameof(commandContext));
                }

                command.Execute(requestContext);
            }
            else
            {
                _logger.LogWarning($"The msg' command {requestContext.Request.Command} was not found.");
            }
        }
    }
}
