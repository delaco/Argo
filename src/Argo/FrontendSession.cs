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
        public virtual string Key { get; set; }

        public string FrontendId { get; set; }

        public string Host { get; set; }

        public DateTime CreateTime { get; }

        public DateTime LastAccessTime
        {
            get;
            set;
        }

        public FrontendSession()
        {
            CreateTime = DateTime.Now;
            LastAccessTime = DateTime.Now;
        }
    }
}
