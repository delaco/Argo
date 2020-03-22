using System;
using System.Collections.Concurrent;

namespace Argo.Internal
{
    internal class ClientWaits
    {
        private readonly ConcurrentDictionary<string, IMessageHandler> _waits = new ConcurrentDictionary<string, IMessageHandler>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="messageHandler"></param>
        public void Start(string key, IMessageHandler messageHandler)
        {
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
        public void Set(string key, IMessage response)
        {
            if (_waits.TryGetValue(key, out IMessageHandler messageHandler))
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
            if (_waits.TryGetValue(key, out IMessageHandler messageHandler))
            {
                messageHandler.AutoResetEvent.WaitOne(TimeSpan.FromSeconds(5));
                _waits.TryRemove(key, out IMessageHandler nettyMessageHandler);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
