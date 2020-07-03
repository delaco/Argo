using Argo.AssemblyParts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Argo.Commands
{
    public class CommandDescriptorCollectionProvider : ICommandDescriptorCollectionProvider
    {
        private AssemblyPartManager _assemblyPartManager;
        private CommandDescriptorCollection _collection;

        public CommandDescriptorCollectionProvider(AssemblyPartManager assemblyPartManager)
        {
            _assemblyPartManager = assemblyPartManager;
        }

        /// <summary>
        /// Returns a cached command of <see cref="CommandDescriptor" />.
        /// </summary>
        public CommandDescriptorCollection CommandDescriptors
        {
            get
            {
                if (_collection == null)
                {
                    UpdateCollection();
                }

                return _collection;
            }
        }

        private void UpdateCollection()
        {
            var feature = new CommandFeature();
            _assemblyPartManager.PopulateFeature(feature);

            var results = new List<CommandDescriptor>();
            var commandTypes = feature.Commands;
            foreach (var typeInfo in commandTypes)
            {
     
                var attribute = typeInfo.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    results.Add(new CommandDescriptor()
                    {
                        // Key = attribute.Id,
                        Name = typeInfo.Name,
                        CommandTypeInfo = typeInfo
                    });
                }
            }

            _collection = new CommandDescriptorCollection(new ReadOnlyCollection<CommandDescriptor>(results));
        }
    }
}