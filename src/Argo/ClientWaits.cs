using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace Argo
{
    public class ClientWaits
    {
        private readonly ConcurrentDictionary<string, ClientObject> _waits = new ConcurrentDictionary<string, ClientObject>();

        public void Start(string key)
        {
            if (!_waits.TryAdd(key, new ClientObject()))
            {
                throw new Exception();
            }
        }

        public void Set(string key, IMessage response)
        {
            var obj = _waits[key];
            obj.Response = response;
            obj.AutoResetEvent.Set();
        }

        public ClientObject Wait(string key)
        {
            var obj = _waits[key];
            obj.AutoResetEvent.WaitOne(TimeSpan.FromSeconds(5));
            _waits.TryRemove(key, out _);

            return obj;
        }
    }
}
