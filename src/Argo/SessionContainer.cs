using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Argo
{
    public class SessionContainer<T>
        where T : Session
    {
        public SessionContainer()
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
            return Members.ContainsKey(id);
        }

        public void Set(string index, T value)
        {
            Members[index] = value;
        }

        public T Get(string index)
        {
            if (Members.TryGetValue(index, out T data))
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
            Members.TryRemove(id, out _);
        }
    }
}
