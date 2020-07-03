using System;
using System.Collections.Generic;
using System.Text;

namespace Argo.Infrastructure
{
    public class ActionInvokerProviderContext
    {
        public ActionInvokerProviderContext(ActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            ActionContext = actionContext;
        }

        public ActionContext ActionContext { get; }

        public IActionInvoker Result { get; set; }
    }
}
