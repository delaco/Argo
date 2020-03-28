using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Argo
{
    public interface IFrontendSessionBuilder
    {
        FrontendSession Build(string userId);
    }

    public class FrontendSessionBuilder : IFrontendSessionBuilder
    {
        private ServerOptions _options;

        public FrontendSessionBuilder(IOptions<ServerOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public FrontendSession Build(string userId)
        {
            return new FrontendSession();
        }
    }
}
