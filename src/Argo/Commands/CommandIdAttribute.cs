using System;

namespace Argo.Commands
{
    /// <summary>
    /// User this attribute to set the properties of command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CommandIdAttribute : Attribute
    {
        /// <summary>
        /// The value which key a command handler
        /// </summary>
        public int Id { get; }

        public CommandIdAttribute(int id)
        {
            Id = id;
        }
    }
}
