using System;
using System.Collections.Generic;
using System.Text;

namespace Cli.Toolkit.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class FlagNameAttribute : Attribute {

    public FlagNameAttribute(string name) {
        Name = name;
    }

    public FlagNameAttribute(string name, string shortName) {
        Name = name;
        ShortName = shortName;
    }

    public string Name { get; }

    public string ShortName { get; set; } = "";
}
