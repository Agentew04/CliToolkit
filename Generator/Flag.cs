using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Generator; 
public sealed class Flag {
    public string Name { get; set; }
    public string ShortName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsOptional { get; set; } = false;

    public bool HasValue { get; set; } = false;

    public PropertyInfo Property { get; set; }

    public static bool TryGetFlagValue(string[] args, Flag flag, out string value) {
        value = "";
        if (!flag.HasValue)
            return false;
        if (args == null || args.Length == 0)
            return false;
        if (!Array.Exists(args, x => flag.Name == x || flag.ShortName == x))
            return false;

        int flagindex = -1;
        for (int i = 0; i < args.Length; i++) {
            if (args[i] == flag.ShortName || args[i] == flag.Name) {
                flagindex = i;
                break;
            }
        }
        if (flagindex == args.Length - 1)
            return false;

        // flag not found
        if (flagindex == -1)
            return false;

        value = args[flagindex + 1];

        // check if value is not other flag( has no value)
        if (value.StartsWith("-") || value.StartsWith("--"))
            return false;

        return true;
    }

    public static bool HasFlag(string[] args, Flag flag) {
        if (args == null || args.Length == 0)
            return false;
        return Array.Exists(args, x => x == flag.Name || x == flag.ShortName);
    }
}
