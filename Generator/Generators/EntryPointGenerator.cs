using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cli.Toolkit.Generators;

[Generator]
public sealed class EntryPointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

        // isso pega apenas metodos que tem o atributo EntryPoint
        var methods = context.SyntaxProvider
            .CreateSyntaxProvider(predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                                  transform: static (ctx, _) => GetTargetForGeneration(ctx));

        // isso junta eles e a compilacao? nao sei
        var compilationAndEnums = context.CompilationProvider.Combine(methods.Collect());

        // isso cria as saidas pra cada classe
        context.RegisterSourceOutput(compilationAndEnums,
            (spc, source) => Execute(source.Left, source.Right, spc));
    }

    public static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
    {
        return syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax &&
            methodDeclarationSyntax.AttributeLists.Count > 0 &&
            methodDeclarationSyntax.AttributeLists.Any(x =>
                x.Attributes.Any(x =>
                    x.Name.ToString() == "EntryPoint" || x.Name.ToString() == "EntryPointAttribute"));
    }

    public static MethodDeclarationSyntax GetTargetForGeneration(GeneratorSyntaxContext context) => (MethodDeclarationSyntax)context.Node;

    public void Execute(Compilation compilation,
        ImmutableArray<MethodDeclarationSyntax> methods,
        SourceProductionContext context)
    {

        if (methods.Length > 1)
        {
            var error = Diagnostic.Create(DiagnosticDescriptors.MultipleEntryPointsMessage,
                               methods[1].GetLocation());
            context.ReportDiagnostic(error);
            return;
        }
        if (methods.Length <= 0)
        {
            return;
        }
        MethodDeclarationSyntax method = methods[0];

        var parameters = method.ParameterList;
        if (parameters.Parameters.Count > 2)
        {
            var error = Diagnostic.Create(DiagnosticDescriptors.MultipleParametersMesage,
                               methods[1].GetLocation());
            context.ReportDiagnostic(error);
            return;
        }
        var configType = parameters.Parameters[0];

        var model = compilation.GetSemanticModel(method.SyntaxTree);


        var methodSymbol = model.GetDeclaredSymbol(method)!;
        var className = methodSymbol.ContainingType.Name;
        var classNamespace = methodSymbol.ContainingNamespace?.ToDisplayString();
        string sourceCode = CreateCode(classNamespace, className, method.Identifier.Text, configType.Type, compilation);

        context.AddSource($"{className}Main.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private static string CreateCode(string @namespace, string @class, string method, TypeSyntax configType, Compilation compilation)
    {
        var code = $$"""
        using Generator;

        namespace {{@namespace}};

        public partial class {{@class}}{
            public static void Main(string[] args){
                System.Console.WriteLine("Hello Main From Generator!");
                {{configType.ToFullString()}} config = new();
                {{CreateFlagsCode(configType, compilation)}}
                {{ParseFlagsCode()}}
                {{method}}(config, args);
            }
        }
        """;
        return code;
    }

    private static string CreateFlagsCode(TypeSyntax type, Compilation compilation)
    {
        StringBuilder sb = new($$"""
        List<Flag> flags = new();

        """);
        var members = (compilation.GetSemanticModel(type.SyntaxTree).GetDeclaredSymbol(type) as ITypeSymbol).GetMembers();
        foreach (var member in members)
        {
            //if(member is not IPropertySymbol property) continue;
            //new Flag() {
            //    Description = "",
            //    HasValue = false,
            //    IsOptional = false,
            //    Name = "",
            //    Property = null,
            //    ShortName = ""
            //};
            //var attributes = property.GetAttributes();
            //var flagName = attributes.FirstOrDefault(x => x.AttributeClass?.Name == "FlagNameAttribute");
            //var optional = attributes.FirstOrDefault(x => x.AttributeClass?.Name == "OptionalAttribute");
            //var required = attributes.FirstOrDefault(x => x.AttributeClass?.Name == "RequiredAttribute");
            //var description = attributes.FirstOrDefault(x => x.AttributeClass?.Name == "DescriptionAttribute");
            //var parameters = attributes.FirstOrDefault(x => x.AttributeClass?.Name == "ParametersAttribute");

            //var name = flagName?.ConstructorArguments[0].Value?.ToString() ?? property.Name;
            //var shortName = flagName?.ConstructorArguments[1].Value?.ToString() ?? "";
            //var isOptional = optional != null;
            //var isRequired = required != null;
            //var hasDescription = description != null;

            //sb.Append($$"""
            //    flags.Add(new Flag(){
            //        {{(hasDescription ? $"Description = {description}," : "")}}
            //        HasValue = false,
            //        IsRequired = false,
            //        Name = "",
            //        Property = null,
            //        ShortName = ""
            //    });
            //    """);
        }

        return sb.ToString();
    }

    private static string ParseFlagsCode()
    {
        var source = $$"""
        List<Flag> flags = new(); // ...

        foreach(Flag flag in flags) {
            bool has;
            string value = "";
            if (flag.HasValue) {
                has = Flag.TryGetFlagValue(args, flag, out value);
            } else {
                has = Flag.HasFlag(args, flag);
            }

            if (!has && flag.IsRequired) {
                // runtime error, warn user
                // issue help menu
                break;
            }

            // parse values and set in flag
            if (has && flag.HasValue) {
                flag.Property.SetValue(config, value);
            }

            if (!flag.HasValue) {
                flag.Property.SetValue(config, has);
            }
        }
        """;


        return source;
    }
}
