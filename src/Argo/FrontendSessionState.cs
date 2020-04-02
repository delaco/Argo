using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace Argo
{
    public class FrontendSessionState<TSession> where TSession : FrontendSession
    {
        private DistributedCacheEntryOptions _distributedCacheEntryOptions;

        public IDistributedCache DistributedCache { get; }

        public ISerializer Serializer { get; }

        public FrontendSessionState(IDistributedCache distributedCache, ISerializer serializer)
        {
            this.DistributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

            this.Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _distributedCacheEntryOptions = new DistributedCacheEntryOptions()
            {
                SlidingExpiration = new TimeSpan(hours: 24, minutes: 0, seconds: 0)
            };
        }

        public TSession Get(string key)
        {
            var bytes = this.DistributedCache.Get(key);
            return Serializer.Deserialize<TSession>(bytes);
        }

        public void Add(TSession session)
        {
            var bytes = Serializer.Serialize(session);
            DistributedCache.Set(session.Key, bytes, _distributedCacheEntryOptions);
        }

        public void Remove(string key)
        {
            DistributedCache.Remove(key);
        }
    }
}
