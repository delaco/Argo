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
        public FrontendSession Build(string userId)
        {
            return new FrontendSession();
        }
    }
}
