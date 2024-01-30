# Cli.Toolkit

This project is mainly composed of a source generator that helps
the developer to create a command line interface for a .NET Core
application. It also contains many useful classes and methods.

## CLI Helper

### Installation

The package is available on NuGet [here](https://www.nuget.org/packages/Cli.Toolkit/).

### Usage

The simplest program consists of a **partial** class with the `CLIProgram`
attribute and a method with the `Command` attribute.

```cs
using CliToolkit.Attributes;

[CliProgram]
public partial class MyCliTool {
    [DefaultCommand]
    [Command]
    public void Add(Config args) {
        // your command here
    }
}
```

The above code would be called like this:

```sh
$ myclitool add
```
or, as there is a command with the `DefaultCommand` attribute:
```sh
$ myclitool
```

> **Note:** Only one method/command can have the `DefaultCommand` attribute.

## Source Generators

Many useful source generators are present, and automate the implementation
of features, such as:
* Builder Pattern
* Lazy Singleton
* Thread Safety for a field 

### Usage

The generators are located in the namespace `Cli.Toolkit.Generators` and
the attributes to trigger them are normally located in the namespace 
`Cli.Toolkit.Attributes`.

```csharp
using Cli.Toolkit.Generators;
using Cli.Toolkit.Attributes;

// partial modifier is required to use source generators
public partial class Program {
    
    // these fields are not required to be static,
    // but is needed if used directly in the Main method(static)
    
    [Lazy]
    private static Config? cfg; // Generates the Cfg property
    
    [ThreadSafe]
    private static int _number = 5; // Generates the Number property
    
    public static void Main() {
        // use them here
        // ...
    }
}
```

```csharp
using Cli.Toolkit.Generators;
using Cli.Toolkit.Attributes;

// annotating the class
// both properties and fields are supported
[Buildable]
public class Hamburger {
    
    public List<string> Ingredients { get; set; } = new();
    
    public int price;
}

// using the new builder
Hamburger hamburger = new HamburgerBuilder()
    .WithIngredients([ "Bread", "Meat", "Cheese" ])
    .WithPrice(5)
    .Build();
```
