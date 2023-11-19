using System;

namespace Generator.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class OptionalAttribute : Attribute {
}