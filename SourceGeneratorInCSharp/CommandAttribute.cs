
namespace Generator.Attributes;
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CommandAttribute : Attribute {
    readonly string positionalString;

    public CommandAttribute(string name) {
        positionalString = name;
    }

    public string Name {
        get { return positionalString; }
    }
}