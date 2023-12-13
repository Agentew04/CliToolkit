using System;

namespace Cli.Toolkit.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class OptionalAttribute : Attribute {
}