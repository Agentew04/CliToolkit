using System;
using System.Collections.Generic;
using System.Text;

namespace Generator.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class DescriptionAttribute : Attribute {

    public DescriptionAttribute(string description)
    {
        this.Description = description;
    }

    public string Description { get; }
}
