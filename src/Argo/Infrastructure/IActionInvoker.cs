using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Argo.Infrastructure
{
    /// <summary>
    /// Defines an interface for invoking an MVC action.
    /// </summary>
    /// <remarks>
    /// An <see cref="IActionInvoker"/> is created for each request the MVC handles by querying the set of
    /// <see cref="IActionInvokerProvider"/> instances. See <see cref="IActionInvokerProvider"/> for more information. 
    /// </remarks>
    public interface IActionInvoker
    {
        /// <summary>
        /// Invokes an MVC action.
        /// </summary>
        /// <returns>A <see cref="Task"/> which will complete when action processing has completed.</returns>
        Task InvokeAsync();
    }
}
