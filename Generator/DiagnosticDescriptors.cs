using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generator; 
public static class DiagnosticDescriptors {
    public static readonly DiagnosticDescriptor MultipleEntryPointsMessage
        = new("ERR001",                                        // id
            "Multiple entry points",                                      // title
            "There was more than one EntryPoints", // message
            "Generator",                                       // category
            DiagnosticSeverity.Error,
            true);

    public static readonly DiagnosticDescriptor MultipleParametersMesage
       = new("ERR002",                                        // id
           "Multiple Parameters",                                      // title
           "Your entry point should have only one parameter", // message
           "Generator",                                       // category
           DiagnosticSeverity.Error,
           true);
}
