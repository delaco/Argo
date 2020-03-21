using System;
using System.Collections.Generic;
using System.Reflection;

namespace Argo.Utils
{
    public static class AssemblyUtil
    {
        /// <summary>
        /// Add an extension method to create an instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the base interface.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public static IEnumerable<TInterface> CreateInstance<TInterface>(this Assembly assembly)
            where TInterface : class
        {
            return CreateInstance<TInterface>(assembly, typeof(TInterface));
        }

        /// <summary>
        /// Add an extension method to create an instance.
        /// </summary>
        /// <typeparam name="TInterface">The type of the base interface.</typeparam>
        /// <param name="assembly">The assembly.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static IEnumerable<TInterface> CreateInstance<TInterface>(this Assembly assembly, Type targetType)
            where TInterface : class
        {
            Type[] arrType = assembly.GetExportedTypes();

            var result = new List<TInterface>();
            foreach (var impltType in arrType)
            {
                if (impltType.IsAbstract)
                    continue;

                if (!targetType.IsAssignableFrom(impltType))
                    continue;

                result.Add((TInterface)Activator.CreateInstance(impltType));
            }

            return result;
        }

        public static IEnumerable<Type> GetAssignabledTypes<TInterface>(this Assembly assembly)
            where TInterface : class
        {
            Type targetType = typeof(TInterface);
            Type[] arrType = assembly.GetExportedTypes();

            var result = new List<Type>();
            foreach (var impltType in arrType)
            {
                if (impltType.IsAbstract)
                    continue;

                if (!targetType.IsAssignableFrom(impltType))
                    continue;

                result.Add(impltType);
            }

            return result;
        }

        /// <summary>
        /// Gets the assemblies from string.
        /// </summary>
        /// <param name="assemblyDef">The assembly def.</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssembliesFromString(string assemblyDef)
        {
            return GetAssembliesFromStrings(assemblyDef.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Gets the assemblies from strings.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssembliesFromStrings(string[] assemblies)
        {
            List<Assembly> result = new List<Assembly>(assemblies.Length);

            foreach (var a in assemblies)
            {
                result.Add(Assembly.Load(a));
            }

            return result;
        }
    }
}
