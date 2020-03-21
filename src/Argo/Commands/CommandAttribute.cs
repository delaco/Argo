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

        public CommandAttribute(string key)
        : this(key, false)
        {
        }

        public CommandAttribute(string key, bool invalid)
        {
            Id = (int)key.GetHashCode();
            Invalid = invalid;
        }
    }
}
