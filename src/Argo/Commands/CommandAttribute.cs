using System;

namespace Argo.Commands
{
    /// <summary>
    /// User this attribute to set the properties of command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CommandAttribute : Attribute
    {
        /// <summary>
        /// The value which key a command handler
        /// </summary>
        public int Id { get; }

        public bool Invalid { get; }

        public CommandAttribute(int id)
            : this(id, false)
        {
        }

        public CommandAttribute(int id, bool invalid)
        {
            Id = id;
            Invalid = invalid;
        }
    }
}
