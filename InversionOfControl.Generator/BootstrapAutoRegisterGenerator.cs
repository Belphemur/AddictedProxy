using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace InversionOfControl.Generator;

/// <summary>
/// Source generator that implements <c>IBootstrap.ConfigureServices</c> for classes
/// implementing <c>IBootstrapAutoRegister&lt;TInterface&gt;</c>.
/// </summary>
[Generator]
public class BootstrapAutoRegisterGenerator : IIncrementalGenerator
{
    private const string AutoRegisterInterfaceName = "InversionOfControl.Model.IBootstrapAutoRegister";
    private const string BootstrapInterfaceName = "InversionOfControl.Model.IBootstrap";
    private const string ServiceLifetimeAttributeName = "InversionOfControl.Model.Factory.ServiceLifetimeAttribute";

    /// <summary>
    /// Display format that prefixes all namespace-qualified names with <c>global::</c>,
    /// including type arguments inside generics.
    /// </summary>
    private static readonly SymbolDisplayFormat GlobalPrefixFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all partial class declarations that implement IBootstrapAutoRegister<T>
        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsPartialClassCandidate(node),
                transform: static (ctx, ct) => GetAutoRegisterInfo(ctx, ct))
            .Where(static info => info != null)
            .Select(static (info, _) => info!.Value);

        // Combine with the compilation to find all implementations
        var compilationAndCandidates = context.CompilationProvider.Combine(candidates.Collect());

        context.RegisterSourceOutput(compilationAndCandidates, static (spc, source) =>
        {
            var (compilation, autoRegisters) = source;
            Execute(compilation, autoRegisters, spc);
        });
    }

    private static bool IsPartialClassCandidate(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDecl &&
               classDecl.Modifiers.Any(m => m.Text == "partial");
    }

    private static AutoRegisterInfo? GetAutoRegisterInfo(GeneratorSyntaxContext context, System.Threading.CancellationToken ct)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        if (!(context.SemanticModel.GetDeclaredSymbol(classDecl, ct) is INamedTypeSymbol classSymbol))
            return null;

        // Find IBootstrapAutoRegister<T> interface
        INamedTypeSymbol? autoRegisterInterface = null;
        foreach (var iface in classSymbol.AllInterfaces)
        {
            if (iface.IsGenericType &&
                iface.ConstructedFrom.ContainingNamespace.ToDisplayString() == "InversionOfControl.Model" &&
                iface.ConstructedFrom.Name == "IBootstrapAutoRegister")
            {
                autoRegisterInterface = iface;
                break;
            }
        }

        if (autoRegisterInterface == null)
            return null;

        // Check it also implements IBootstrap
        var implementsBootstrap = classSymbol.AllInterfaces.Any(i =>
            i.ToDisplayString() == BootstrapInterfaceName);

        if (!implementsBootstrap)
            return null;

        // Get the TInterface type argument
        var serviceInterfaceType = autoRegisterInterface.TypeArguments[0];

        // Get the ServiceLifetime from attribute, default to Scoped
        var lifetime = "Scoped";
        foreach (var attr in classSymbol.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() == ServiceLifetimeAttributeName &&
                attr.ConstructorArguments.Length == 1)
            {
                // The enum value name
                var enumValue = attr.ConstructorArguments[0].Value;
                if (enumValue != null)
                {
                    // ServiceLifetime enum: Singleton=0, Scoped=1, Transient=2
                    switch ((int)enumValue)
                    {
                        case 0:
                            lifetime = "Singleton";
                            break;
                        case 1:
                            lifetime = "Scoped";
                            break;
                        case 2:
                            lifetime = "Transient";
                            break;
                    }
                }
            }
        }

        return new AutoRegisterInfo(
            classSymbol.ToDisplayString(),
            classSymbol.Name,
            classSymbol.ContainingNamespace.ToDisplayString(),
            serviceInterfaceType.ToDisplayString(GlobalPrefixFormat),
            serviceInterfaceType.ToDisplayString(),
            lifetime);
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<AutoRegisterInfo> autoRegisters,
        SourceProductionContext context)
    {
        if (autoRegisters.IsDefaultOrEmpty)
            return;

        foreach (var info in autoRegisters.Distinct())
        {
            var serviceInterfaceSymbol = compilation.GetTypeByMetadataName(info.ServiceInterfaceMetadataName);
            if (serviceInterfaceSymbol == null)
                continue;

            // Find all concrete implementations of the interface across the compilation
            var implementations = FindImplementations(compilation, serviceInterfaceSymbol);

            var source = GenerateSource(info, implementations);
            context.AddSource($"{info.ClassName}.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static ImmutableArray<string> FindImplementations(Compilation compilation, INamedTypeSymbol interfaceSymbol)
    {
        var builder = ImmutableArray.CreateBuilder<string>();
        var visitor = new ImplementationFinder(interfaceSymbol, builder);

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();
            visitor.Visit(semanticModel, root);
        }

        builder.Sort();
        return builder.ToImmutable();
    }

    private static string GenerateSource(AutoRegisterInfo info, ImmutableArray<string> implementations)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.Extensions.Configuration;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine();
        sb.AppendLine($"namespace {info.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"partial class {info.ClassName}");
        sb.AppendLine("{");
        sb.AppendLine("    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)");
        sb.AppendLine("    {");

        foreach (var impl in implementations)
        {
            sb.AppendLine($"        services.Add{info.Lifetime}<{info.ServiceInterfaceFullName}, {impl}>();");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Walks all type declarations in a syntax tree to find concrete implementations.
    /// </summary>
    private class ImplementationFinder
    {
        private readonly INamedTypeSymbol _interfaceSymbol;
        private readonly ImmutableArray<string>.Builder _builder;

        public ImplementationFinder(INamedTypeSymbol interfaceSymbol, ImmutableArray<string>.Builder builder)
        {
            _interfaceSymbol = interfaceSymbol;
            _builder = builder;
        }

        public void Visit(SemanticModel semanticModel, SyntaxNode root)
        {
            foreach (var node in root.DescendantNodes())
            {
                if (node is ClassDeclarationSyntax classDecl)
                {
                    if (!(semanticModel.GetDeclaredSymbol(classDecl) is INamedTypeSymbol symbol) || symbol.IsAbstract)
                        continue;

                    if (ImplementsInterface(symbol, _interfaceSymbol))
                    {
                        _builder.Add(symbol.ToDisplayString(GlobalPrefixFormat));
                    }
                }
            }
        }

        private static bool ImplementsInterface(INamedTypeSymbol type, INamedTypeSymbol interfaceSymbol)
        {
            foreach (var iface in type.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(iface, interfaceSymbol))
                    return true;
            }

            return false;
        }
    }

    private readonly struct AutoRegisterInfo : System.IEquatable<AutoRegisterInfo>
    {
        public string FullName { get; }
        public string ClassName { get; }
        public string Namespace { get; }
        public string ServiceInterfaceFullName { get; }
        public string ServiceInterfaceMetadataName { get; }
        public string Lifetime { get; }

        public AutoRegisterInfo(string fullName, string className, string ns, string serviceInterfaceFullName, string serviceInterfaceMetadataName, string lifetime)
        {
            FullName = fullName;
            ClassName = className;
            Namespace = ns;
            ServiceInterfaceFullName = serviceInterfaceFullName;
            ServiceInterfaceMetadataName = serviceInterfaceMetadataName;
            Lifetime = lifetime;
        }

        public bool Equals(AutoRegisterInfo other)
        {
            return FullName == other.FullName;
        }

        public override bool Equals(object obj)
        {
            return obj is AutoRegisterInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return FullName != null ? FullName.GetHashCode() : 0;
        }
    }
}
