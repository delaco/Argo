using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;

namespace Argo.Internal
{
    public class ClientWaits
    {
        private readonly ConcurrentDictionary<string, DotNettyMessageHandler> _waits = new ConcurrentDictionary<string, DotNettyMessageHandler>();

        public ClientWaits()
        { 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="messageHandler"></param>
        public void Start(string key, DotNettyMessageHandler messageHandler)
        {
            Contract.Requires(messageHandler != null);
            messageHandler.AyscResponse = null;
            if (!_waits.TryAdd(key, messageHandler))
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="response"></param>
        public void Set(string key, IPacket response)
        {
            if (_waits.TryGetValue(key, out DotNettyMessageHandler messageHandler))
            {
                messageHandler.AyscResponse = response;
                messageHandler.AutoResetEvent.Set();
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Wait(string key)
        {
            if (_waits.TryGetValue(key, out DotNettyMessageHandler messageHandler))
            {
                messageHandler.AutoResetEvent.WaitOne(TimeSpan.FromSeconds(5));
                _waits.TryRemove(key, out _);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
