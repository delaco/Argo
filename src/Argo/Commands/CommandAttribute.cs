using System;

namespace Argo.Commands
{
    /// <summary>
    /// User this attribute to set the properties of command
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// The value which key a command handler
        /// </summary>
        public uint Id { get; }

        public bool Invalid { get; }

        public CommandAttribute(uint key)
            : this(key, false)
        {
        }

        public CommandAttribute(uint id, bool invalid)
        {
            Id = id;
            Invalid = invalid;
        }

        public CommandAttribute(string key)
        : this(key, false)
        {
        }

        public CommandAttribute(string key, bool invalid)
        {
            Id = (uint)key.GetHashCode();
            Invalid = invalid;
        }
    }
}
