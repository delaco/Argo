using System;
using System.Collections.Generic;
using System.Text;

namespace Argo
{
    public interface ISession
    {
        DateTime CreateTime { get; }

        DateTime LastAccessTime { get; set; }
    }
}
