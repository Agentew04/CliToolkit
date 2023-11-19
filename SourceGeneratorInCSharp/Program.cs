using Generator;
using Generator.Attributes;
using System.Diagnostics;
using System.Reflection;

namespace SourceGeneratorInCSharp;

[CliProgram]
public partial class HelloWorld {

    public static void Main(string[] args) {
        var sw = Stopwatch.StartNew();
        var program = new HelloWorld();

        string command = "";
        bool defaultCommand = false;
        if(args.Length == 0) {
            defaultCommand = true;
        } else {
            command = args[0];
        }

        MethodInfo? method;
        // get the method for this command
        if (!defaultCommand) {
            method = program
                .GetType()
                .GetMethods()
                .Where(x => x.GetCustomAttribute<CommandAttribute>() is not null)
                .FirstOrDefault(x => x.GetCustomAttribute<CommandAttribute>()!.Name == command);
        } else {
            method = program
                .GetType()
                .GetMethods()
                .Where(x => x.GetCustomAttribute<CommandAttribute>() is not null)
                .FirstOrDefault(x => x.GetCustomAttribute<DefaultCommandAttribute>() is not null);
        }

        if(method is null) {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid command!");
            Console.ForegroundColor = color;
            return;
        }

        IEnumerable<Type> argParams = method
            .GetParameters()
            .Select(x => x.ParameterType)
            .Where(x => x.GetCustomAttribute<ArgumentsAttribute>() is not null);

        // collect all flags
        var flagsType = CollectFlags(argParams);

        object?[] parameters = GetCommandParameters(method, args, flagsType);

        method.Invoke(program, parameters);
        sw.Stop();
        Console.WriteLine($"Finished in {sw.Elapsed}ms");
    }

    private static Dictionary<Type, List<Flag>> CollectFlags(IEnumerable<Type> types) {
        Dictionary<Type, List<Flag>> flags = new();

        foreach(Type type in types) {
            flags[type] = new List<Flag>();
            var properties = type.GetProperties();

            foreach(var property in properties) {
                bool ignored = property.GetCustomAttribute<IgnoreAttribute>() is not null;
                if (ignored)
                    continue;

                string fullname;
                string shortname;
                FlagNameAttribute? flagNameAttribute = property.GetCustomAttribute<FlagNameAttribute>();
                if (flagNameAttribute is not null) {
                    fullname = flagNameAttribute.Name;
                    shortname = flagNameAttribute.ShortName;
                } else {
                    fullname = property.Name;
                    shortname = "";
                }

                bool isOptional = property.GetCustomAttribute<OptionalAttribute>() is not null;

                bool hasValue = property.PropertyType != typeof(bool);

                flags[type].Add(new Flag {
                    Name = fullname,
                    ShortName = shortname,
                    Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "",
                    IsOptional = isOptional,
                    HasValue = hasValue,
                    Property = property
                });
            }
        }
        return flags;
    }

    private static object?[] GetCommandParameters(MethodInfo command,
                                                 string[] args,
                                                 Dictionary<Type, List<Flag>> flagsType) {
        object?[] parameters = new object?[command.GetParameters().Length];
        int i = 0;
        foreach (var kvp in flagsType) {
            Type type = kvp.Key;
            List<Flag> flags = kvp.Value;
            var obj = Activator.CreateInstance(type);

            // parse the flags for this class
            foreach (var flag in flags) {
                if (!flag.HasValue) { // is boolean
                    bool flagvalue = Flag.HasFlag(args, flag);
                    flag.Property.SetValue(obj, flagvalue);
                    continue;
                }

                bool hasValue = Flag.TryGetFlagValue(args, flag, out string value);
                if (flag.IsOptional && !hasValue) {
                    if (flag.Property.PropertyType.IsValueType) {
                        flag.Property.SetValue(obj, Activator.CreateInstance(flag.Property.PropertyType));
                    } else {
                        flag.Property.SetValue(obj, null);
                    }
                    continue;
                }
                flag.Property.SetValue(obj, value);
            }

            parameters[i] = obj;
            i++;
        }
        return parameters;
    }

    [Init]
    public void Init() {
        // init shared stuff
    }

    [DefaultCommand]
    [Command("add")]
    public void Add(Config args) {

    }
    
}
