using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Cli.Toolkit.Generators;

[Generator]
public class BuildableGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // get all fields with the Attribute
        var fields = context.SyntaxProvider
            .CreateSyntaxProvider(predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                                  transform: static (ctx, _) => GetTargetForGeneration(ctx));

        var compAndFields = context.CompilationProvider.Combine(fields.Collect());

        context.RegisterSourceOutput(compAndFields,
            (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode) {
        if (syntaxNode is not ClassDeclarationSyntax && syntaxNode is not StructDeclarationSyntax) {
            return false;
        }

        TypeDeclarationSyntax typeDeclarationSyntax = (TypeDeclarationSyntax)syntaxNode;
        
        if(typeDeclarationSyntax.AttributeLists.Count == 0) {
            return false;
        }
        return typeDeclarationSyntax.AttributeLists.Count > 0
            && typeDeclarationSyntax.AttributeLists.Any(x =>
                x.Attributes.Any(x => x.Name.ToString() == "Buildable"));
    }

    private static TypeDeclarationSyntax GetTargetForGeneration(GeneratorSyntaxContext ctx) {
        return (TypeDeclarationSyntax)ctx.Node;
    }

    private void Execute(Compilation compilation, ImmutableArray<TypeDeclarationSyntax> types, SourceProductionContext context) {

        foreach(var type in types) {
            SemanticModel model = compilation.GetSemanticModel(type.SyntaxTree);

            // get all members(properties and fields)
            var members = type.Members
                .Where(x => x is PropertyDeclarationSyntax || x is FieldDeclarationSyntax);
            
            INamedTypeSymbol typeSymbol = model.GetDeclaredSymbol(type);
            string namespaceName = typeSymbol.ContainingNamespace.ToString();

            StringBuilder sb = new();

            sb.AppendLine($"namespace {namespaceName};");
            sb.AppendLine();
            sb.AppendLine($"public class {type.Identifier.Text}Builder {{");
            sb.AppendLine();
            foreach(var member in members) {
                string memberName = member switch {
                    PropertyDeclarationSyntax p => p.Identifier.Text,
                    FieldDeclarationSyntax f => f.Declaration.Variables[0].Identifier.Text,
                    _ => throw new NotImplementedException()
                };

                string memberType = member switch {
                    PropertyDeclarationSyntax p => p.Type.ToString(),
                    FieldDeclarationSyntax f => f.Declaration.Type.ToString(),
                    _ => throw new NotImplementedException()
                };

                // create the private field
                sb.AppendLine($"    private {memberType} {memberName};");
                sb.AppendLine();

                // create the with method
                sb.AppendLine($"    public {type.Identifier.Text}Builder With{memberName.Substring(0, 1).ToUpper() + memberName.Substring(1)}({memberType} {memberName}) {{");
                sb.AppendLine($"        this.{memberName} = {memberName};");
                sb.AppendLine($"        return this;");
                sb.AppendLine($"    }}");
                sb.AppendLine();
            }

            // create the build method
            sb.AppendLine($"    public {type.Identifier.Text} Build() {{");
            sb.AppendLine($"        return new() {{");
            foreach(var member in members) {
                string memberName = member switch {
                    PropertyDeclarationSyntax p => p.Identifier.Text,
                    FieldDeclarationSyntax f => f.Declaration.Variables[0].Identifier.Text,
                    _ => throw new NotImplementedException()
                };

                sb.AppendLine($"            {memberName} = this.{memberName},");
            }
            sb.AppendLine($"        }};");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            sb.AppendLine();

            string sourceCode = sb.ToString();
            context.AddSource($"{type.Identifier.Text}Builder.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));
        }
    }
}
