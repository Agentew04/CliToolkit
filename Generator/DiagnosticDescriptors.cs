using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cli.Toolkit;
public static class DiagnosticDescriptors {
    public static readonly DiagnosticDescriptor MultipleEntryPointsMessage
        = new("CLI001",                                        // id
            "Multiple entry points",                                      // title
            "There was more than one EntryPoints", // message
            "Generator",                                       // category
            DiagnosticSeverity.Error,
            true);

    public static readonly DiagnosticDescriptor MultipleParametersMesage
       = new("CLI002",                                        // id
           "Multiple Parameters",                                      // title
           "Your entry point should have only one parameter", // message
           "Generator",                                       // category
           DiagnosticSeverity.Error,
           true);

    public static readonly DiagnosticDescriptor NotPartialClassMessage
        = new("CLI003",
            "Class is not partial",
            "The declaring class should be partial to enable extensions.",
            "Generator",
            DiagnosticSeverity.Error,
            true);

    public static readonly DiagnosticDescriptor NotNullableTypeMessage
        = new("CLI004",
            "Type is not nullable",
            "The type should be nullable",
            "Generator",
            DiagnosticSeverity.Error,
            true);
}
