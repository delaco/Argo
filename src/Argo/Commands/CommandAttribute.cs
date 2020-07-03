using System;

namespace Argo.Commands
{
    /// <summary>
    /// User this attribute to set the properties of command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
    }
}
