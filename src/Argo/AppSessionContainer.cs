using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Argo
{
    public class AppSessionContainer<T>
        where T : AppSession
    {
        public AppSessionContainer()
        {
            Members = new ConcurrentDictionary<string, T>();
        }

        public ConcurrentDictionary<string, T> Members { get; private set; }

        public T this[string id]
        {
            get { return Members[id]; }
        }

        public bool Contains(string id)
        {
            Contract.Requires(id != null);
            return Members.ContainsKey(id);
        }

        public void Set(string id, T value)
        {
            Contract.Requires(id != null);
            Members.AddOrUpdate(id, value, (k1, v1) => value);
        }

        public T Get(string id)
        {
            Contract.Requires(id != null);
            if (Members.TryGetValue(id, out T data))
                return data;

            return default;
        }

        public IEnumerable<T> Find(Predicate<T> critera = null)
        {
            var enumerator = Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var s = enumerator.Current.Value;

                /* todo:
                if (s.State != SessionState.Connected)
                    continue;
                */

                if (critera == null || critera(s))
                    yield return s;
            }
        }

        public void Remove(string id)
        {
            Contract.Requires(id != null);
            Members.TryRemove(id, out _);
        }
    }
}
