using System;
using System.Collections.Generic;
using System.Text;

namespace Cli.Toolkit.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class ParametersAttribute : Attribute {
}
