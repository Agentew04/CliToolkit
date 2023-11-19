using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGeneratorInCSharp {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ArgumentsAttribute : Attribute{
    }
}
