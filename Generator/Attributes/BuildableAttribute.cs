using System;

namespace Cli.Toolkit.Attributes;

/// <summary>
/// An attribute that points which class should be used as a builder for the target class.
/// </summary>
/// <typeparam name="T">The class that is the builder.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class BuildableAttribute : Attribute {
}
