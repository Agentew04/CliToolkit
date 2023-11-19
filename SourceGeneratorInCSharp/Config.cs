using Generator.Attributes;

namespace SourceGeneratorInCSharp {
    [Arguments]
    public class Config {
        [FlagName("verbose", "v")]
        public bool PrintMore { get; init; }

        [FlagName("user")]
        [Optional]
        [Description("the name of the user")]
        public string Username { get; init; } = "";

        [Parameters]
        public List<string> Parameters { get; init; } = new();

        [Ignore]
        public int Number { get; init; } = 0;
    }
}
