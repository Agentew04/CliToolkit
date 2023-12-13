using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Cli.Toolkit.Generators;

[Generator]
public class LazyGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        // get all fields with the LazyAttribute
        var fields = context.SyntaxProvider
            .CreateSyntaxProvider(predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                                  transform: static (ctx, _) => GetTargetForGeneration(ctx));

        var compAndFields = context.CompilationProvider.Combine(fields.Collect());

        context.RegisterSourceOutput(compAndFields,
            (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode) {
        if (syntaxNode is not FieldDeclarationSyntax) {
            return false;
        }
        FieldDeclarationSyntax fieldDeclarationSyntax = (FieldDeclarationSyntax)syntaxNode;
        if (fieldDeclarationSyntax.AttributeLists.Count == 0) {
            return false;
        }


        return fieldDeclarationSyntax.AttributeLists.Count > 0
            && fieldDeclarationSyntax.AttributeLists
            .Any(x =>
                x.Attributes.Any(x => x.Name.ToString() == "Lazy"));
    }

    private static FieldDeclarationSyntax GetTargetForGeneration(GeneratorSyntaxContext ctx) {
        return (FieldDeclarationSyntax)ctx.Node;
    }


    private void Execute(Compilation compilation, ImmutableArray<FieldDeclarationSyntax> fields, SourceProductionContext context) {
        foreach (var field in fields) {
            SemanticModel model = compilation.GetSemanticModel(field.SyntaxTree);
            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)field.Parent;
            INamespaceSymbol namespaceSymbol = model.GetDeclaredSymbol(classDeclaration).ContainingNamespace;
            TypeSyntax type = field.Declaration.Type;
            string backingFieldName = field.Declaration.Variables[0].Identifier.ValueText;
            string propertyName = backingFieldName.Substring(0, 1).ToUpper() + backingFieldName.Substring(1);
            bool isStatic = field.Modifiers.Any(SyntaxKind.StaticKeyword);

            // check if class is partial
            if (!classDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)) {
                var error = Diagnostic.Create(DiagnosticDescriptors.NotPartialClassMessage,
                                                      classDeclaration.GetLocation());
                context.ReportDiagnostic(error);
                continue;
            }

            // check if backing field is nullable
            if (type.IsNotNull) {
                var error = Diagnostic.Create(DiagnosticDescriptors.NotNullableTypeMessage,
                                                                         type.GetLocation());
                context.ReportDiagnostic(error);
                continue;
            }

            string nonNullableTypeName = type.ToString().Substring(0, type.ToString().Length - 1);

            string property = string.Format(propertyTemplate, nonNullableTypeName, propertyName, backingFieldName, isStatic ? "static" : "");

            string sourceCode = string.Format(classTemplate, namespaceSymbol.ToDisplayString(), classDeclaration.Identifier.Text, property);

            context.AddSource($"{classDeclaration.Identifier.Text}.{propertyName}.Lazy.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private const string classTemplate = """
        namespace {0};
        public partial class {1} {{
            {2}
        }}
        """;
    private const string propertyTemplate = """
        public {3} {0} {1} {{
            get {{
                {2} ??= new {0}();
                return {2};
            }}
            set {{
                if({2} != value) {{
                    {2} = value;
                }}
            }}
        }}
        """;
}