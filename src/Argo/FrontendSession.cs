using System.Net;
using System.Threading.Tasks;
using System;

namespace Argo
{
    /// <summary>
    /// todo:
    /// </summary>
    public abstract class FrontendSession : ISession
    {
        private DateTime _createTime;
        private DateTime _lastAccessTime;

        public string UId { get; }

        public string Host { get; }

        public DateTime CreateTime => _createTime;

        public DateTime LastAccessTime
        {
            get => _lastAccessTime;
            set => _lastAccessTime = value;
        }

        public FrontendSession()
        {
            _createTime = DateTime.Now;
        }
    }
}
