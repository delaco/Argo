using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace Argo
{
    public class FrontendSessionState
    {
        private readonly IDistributedCache _cache;

        public IDistributedCache DistributedCache => _cache;

        public FrontendSessionState()
        {
            this._cache = null;
        }

        public FrontendSession Get(string userId)
        {
            this._cache.Get(userId);
            return null;
        }
    }
}
