using System;
using System.Collections.Generic;
using System.Text;

namespace Argo.Infrastructure
{
    public class ParameterDescriptor
    {
        public string Name { get; set; }

        public Type ParameterType { get; set; }

        public BindingInfo BindingInfo { get; set; }
    }
}
