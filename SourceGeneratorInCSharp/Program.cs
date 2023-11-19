using Generator;
using Generator.Attributes;
using System.Reflection;

namespace SourceGeneratorInCSharp;

[CliProgram]
public partial class HelloWorld {

    public static void Main(string[] args) {
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
        var flags = CollectFlags(argParams);
    }

    private static List<Flag> CollectFlags(IEnumerable<Type> types) {
        List<Flag> flags = new();

        foreach(Type type in types) {
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

                flags.Add(new Flag {
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

    [Init]
    public void Init() {
        // init shared stuff
    }

    [DefaultCommand]
    [Command("add")]
    public void Add(Config args) {

    }
    
}
