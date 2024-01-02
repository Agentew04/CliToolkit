using Cli.Toolkit.Attributes;
using Cli.Toolkit.Generators;

namespace SourceGeneratorInCSharp {
    [Arguments]
    [Buildable]
    public class Config {
        [FlagName("verbose", "v")]
        public bool PrintMore { get; init; }

        [FlagName("user")]
        [Optional]
        [Description("the name of the user")]
        public string Username { get; init; } = "";

        [Ignore]
        public int Number { get; init; } = 0;

        public Config()
        {
            Console.WriteLine("Config created!");
        }
    }
}
