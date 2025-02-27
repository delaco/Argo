﻿using Argo.Commands;
using Argo.PassThrough;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.Contracts;

namespace Argo
{
    public class MessageRouter : IMessageRouter
    {
        private IServiceProvider _serviceProvider;
        private ICommandDescriptorContainer _commandContainer;
        private ICommandActivator _commandActivator;
        private IPassThroughRuleProvider _passthroughRuleProvider;
        private ILogger<MessageRouter> _logger;

        public MessageRouter(IServiceProvider serviceProvider,
            ICommandDescriptorContainer commandContainer,
            ICommandActivator commandActivator,
            IPassThroughRuleProvider passthroughRuleProvider,
            ILogger<MessageRouter> logger)
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
                try
                {
                    var commandDescriptor = this._commandContainer.Get(requestContext.Request.Command);
                    if (commandDescriptor != null)
                    {
                        var commandContext = new CommandContext(commandDescriptor, _serviceProvider);
                        if ((_commandActivator.Create(commandContext) is ICommand command))
                        {
                            command.Execute(requestContext);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"The msg' command {requestContext.Request.Command} was not found.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"The msg' command {requestContext.Request.Command} catch an error {ex}.");
                }
            }
        }
    }
}