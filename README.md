# Cli.Toolkit

This project is mainly composed of a source generator that helps
the developer to create a command line interface for a .NET Core
application. It also contains many useful classes and methods.

## Installation

The package is available on NuGet [here](https://www.nuget.org/packages/Cli.Toolkit/).

## Usage

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
