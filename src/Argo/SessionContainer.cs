using System.Collections.Generic;

namespace Argo
{
    public class SessionContainer<T>
        where T : Session
    {
        public SessionContainer()
        {
            Members = new Dictionary<string, T>();
        }

        public IDictionary<string, T> Members { get; private set; }

        public T this[string index]
        {
            get { return Members[index]; }
        }

        public bool Contains(string index)
        {
            return Members.ContainsKey(index);
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

        public void Remove(string index)
        {
            Members.Remove(index);
        }
    }
}
