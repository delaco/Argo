using System;

namespace Argo.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NonCommandAttribute : Attribute
    {
    }
}
