using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreAttribute : Attribute {
    }
}
