using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace Argo.Internal
{
    internal class ClientWaits
    {
        private readonly ConcurrentDictionary<string, DotNettyMessageHandler> _waits = new ConcurrentDictionary<string, DotNettyMessageHandler>();

        public void Start(string key, DotNettyMessageHandler dotNettyMessageHandler)
        {
            dotNettyMessageHandler.Response = null;
            if (!_waits.TryAdd(key, dotNettyMessageHandler))
            {
                throw new Exception();
            }
        }

        public void Set(string key, IMessage response)
        {
            if (_waits.TryGetValue(key, out DotNettyMessageHandler dotNettyMessageHandler))
            {
                dotNettyMessageHandler.Response = response;
                dotNettyMessageHandler.AutoResetEvent.Set();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void Wait(string key)
        {
            if (_waits.TryGetValue(key, out DotNettyMessageHandler dotNettyMessageHandler))
            {
                dotNettyMessageHandler.AutoResetEvent.WaitOne(TimeSpan.FromSeconds(5));
                _waits.TryRemove(key, out DotNettyMessageHandler nettyMessageHandler);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
