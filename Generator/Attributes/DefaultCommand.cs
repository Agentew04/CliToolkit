using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.Attributes;
[AttributeUsage(AttributeTargets.Method)]
public sealed class DefaultCommandAttribute : Attribute{
}
