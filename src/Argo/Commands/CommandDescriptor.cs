using System.Collections.Generic;
using System.Reflection;

namespace Argo.Commands
{
    public class CommandDescriptor
    {
        /// <summary>
        /// Gets or sets descriptor's key
        /// </summary>
        public int Key { get; set; }

        /// <summary>
        /// Gets or sets descriptor's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Command 's TypeInfo
        /// </summary>
        public TypeInfo CommandTypeInfo { get; set; }

        public CommandDescriptor()
        {
            Properties = new Dictionary<object, object>();
        }

        ///// <summary>
        ///// The set of filters associated with this command.
        ///// </summary>
        //public IList<FilterDescriptor> FilterDescriptors { get; set; }

        /// <summary>
        /// Stores arbitrary metadata properties associated with the <see cref="CommandDescriptor"/>.
        /// </summary>
        public IDictionary<object, object> Properties { get; }
    }
}