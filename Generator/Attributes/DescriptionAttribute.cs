using System;
using System.Collections.Generic;
using System.Text;

namespace Cli.Toolkit.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class DescriptionAttribute : Attribute {

    public DescriptionAttribute(string description) {
        Description = description;
    }

    public string Description { get; }
}
