using Argo.Commands;
using Argo.PassThrough;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.Contracts;

namespace Argo
{
    public class PacketRouter : IMessageRouter
    {
        private IServiceProvider _serviceProvider;
        private ICommandDescriptorContainer _commandContainer;
        private ICommandActivator _commandActivator;
        private IPassThroughRuleProvider _passthroughRuleProvider;
        private ILogger<PacketRouter> _logger;

        public PacketRouter(IServiceProvider serviceProvider,
            ICommandDescriptorContainer commandContainer,
            ICommandActivator commandActivator,
            IPassThroughRuleProvider passthroughRuleProvider,
            ILogger<PacketRouter> logger)
        {
            _serviceProvider = serviceProvider ?? throw new NullReferenceException(nameof(serviceProvider));
            _commandContainer = commandContainer ?? throw new NullReferenceException(nameof(commandContainer));
            _commandActivator = commandActivator ?? throw new NullReferenceException(nameof(commandActivator));
            _passthroughRuleProvider = passthroughRuleProvider ?? throw new NullReferenceException(nameof(passthroughRuleProvider));
            _logger = logger;
        }

        public virtual void Route(RequestContext requestContext)
        {
            Contract.Requires(requestContext != null);
            var passthroughRule = _passthroughRuleProvider.Create(requestContext);

            if (passthroughRule.IsPassThrough)
            {
            }
            else
            {
                var commandDescriptor = this._commandContainer.Get(requestContext);
                if (commandDescriptor != null)
                {
                    var commandContext = new CommandContext(requestContext, commandDescriptor, _serviceProvider);
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
}