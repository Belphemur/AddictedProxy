using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace InversionOfControl.Generator;

/// <summary>
/// Source generator that auto-registers <c>EnumFactory&lt;TEnum, TService&gt;</c> subclasses
/// and their <c>IEnumService&lt;TEnum&gt;</c> implementations into the DI container.
/// <para>
/// For each concrete factory subclass found in the compilation, this generator
/// produces a partial <c>IBootstrap</c> class with a <c>ConfigureServices</c> method
/// that registers:
/// <list type="bullet">
///   <item>Each concrete <c>IEnumService&lt;TEnum&gt;</c> implementation as its service interface</item>
///   <item>The <c>EnumFactory&lt;TEnum, TService&gt;</c> base type</item>
///   <item>The concrete factory subclass (if distinct from the base)</item>
/// </list>
/// Lifetime is determined by <c>[ServiceLifetime]</c> on the factory class, defaulting to Singleton.
/// </para>
/// </summary>
[Generator]
public class EnumFactoryRegistrationGenerator : IIncrementalGenerator
{
    private const string EnumFactoryFullName = "InversionOfControl.Model.Factory.EnumFactory";
    private const string EnumServiceFullName = "IEnumService";
    private const string EnumServiceNamespace = "InversionOfControl.Model.Factory";
    private const string ServiceLifetimeAttributeName = "InversionOfControl.Model.Factory.ServiceLifetimeAttribute";

    /// <summary>
    /// Display format that prefixes all namespace-qualified names with <c>global::</c>,
    /// including type arguments inside generics.  This avoids ambiguity when a namespace
    /// segment collides with a type name (e.g. <c>Compressor.Factory</c>).
    /// </summary>
    private static readonly SymbolDisplayFormat GlobalPrefixFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all non-partial, non-abstract class declarations that extend EnumFactory<,>
        var factoryCandidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsClassCandidate(node),
                transform: static (ctx, ct) => GetFactoryInfo(ctx, ct))
            .Where(static info => info != null)
            .Select(static (info, _) => info!.Value);

        var compilationAndFactories = context.CompilationProvider.Combine(factoryCandidates.Collect());

        context.RegisterSourceOutput(compilationAndFactories, static (spc, source) =>
        {
            var (compilation, factories) = source;
            Execute(compilation, factories, spc);
        });
    }

    private static bool IsClassCandidate(SyntaxNode node)
    {
        // Match any class that has a base list (potential EnumFactory subclass)
        return node is ClassDeclarationSyntax classDecl &&
               classDecl.BaseList != null &&
               !classDecl.Modifiers.Any(m => m.Text == "abstract");
    }

    private static FactoryInfo? GetFactoryInfo(GeneratorSyntaxContext context, System.Threading.CancellationToken ct)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        if (!(context.SemanticModel.GetDeclaredSymbol(classDecl, ct) is INamedTypeSymbol classSymbol))
            return null;

        if (classSymbol.IsAbstract)
            return null;

        // Check if it extends EnumFactory<TEnum, TService>
        var baseType = classSymbol.BaseType;
        if (baseType == null || !baseType.IsGenericType)
            return null;

        var baseDef = baseType.ConstructedFrom;
        if (baseDef.ContainingNamespace.ToDisplayString() != EnumServiceNamespace ||
            baseDef.Name != "EnumFactory")
            return null;

        // Get TEnum and TService type arguments
        if (baseType.TypeArguments.Length != 2)
            return null;

        var enumType = baseType.TypeArguments[0];
        var serviceInterfaceType = baseType.TypeArguments[1];

        // Read [ServiceLifetime] attribute, default to Singleton
        var lifetime = "Singleton";
        foreach (var attr in classSymbol.GetAttributes())
        {
            if (attr.AttributeClass?.ToDisplayString() == ServiceLifetimeAttributeName &&
                attr.ConstructorArguments.Length == 1)
            {
                var enumValue = attr.ConstructorArguments[0].Value;
                if (enumValue != null)
                {
                    switch ((int)enumValue)
                    {
                        case 0: lifetime = "Singleton"; break;
                        case 1: lifetime = "Scoped"; break;
                        case 2: lifetime = "Transient"; break;
                    }
                }
            }
        }

        return new FactoryInfo(
            concreteFactoryFullName: classSymbol.ToDisplayString(GlobalPrefixFormat),
            concreteFactoryName: classSymbol.Name,
            concreteFactoryNamespace: classSymbol.ContainingNamespace.ToDisplayString(),
            enumTypeFullName: enumType.ToDisplayString(GlobalPrefixFormat),
            serviceInterfaceFullName: serviceInterfaceType.ToDisplayString(GlobalPrefixFormat),
            serviceInterfaceMetadataName: serviceInterfaceType.ToDisplayString(),
            baseFactoryFullName: baseType.ToDisplayString(GlobalPrefixFormat),
            lifetime: lifetime);
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<FactoryInfo> factories,
        SourceProductionContext context)
    {
        if (factories.IsDefaultOrEmpty)
            return;

        foreach (var info in factories.Distinct())
        {
            var serviceInterfaceSymbol = compilation.GetTypeByMetadataName(info.ServiceInterfaceMetadataName);
            if (serviceInterfaceSymbol == null)
                continue;

            // Find all concrete implementations of the service interface
            var implementations = FindImplementations(compilation, serviceInterfaceSymbol);

            var source = GenerateSource(info, implementations);
            context.AddSource($"{info.ConcreteFactoryName}Registration.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static ImmutableArray<string> FindImplementations(Compilation compilation, INamedTypeSymbol interfaceSymbol)
    {
        var builder = ImmutableArray.CreateBuilder<string>();

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var root = syntaxTree.GetRoot();

            foreach (var node in root.DescendantNodes())
            {
                if (node is ClassDeclarationSyntax classDecl)
                {
                    if (!(semanticModel.GetDeclaredSymbol(classDecl) is INamedTypeSymbol symbol) || symbol.IsAbstract)
                        continue;

                    foreach (var iface in symbol.AllInterfaces)
                    {
                        if (SymbolEqualityComparer.Default.Equals(iface, interfaceSymbol))
                        {
                            builder.Add(symbol.ToDisplayString(GlobalPrefixFormat));
                            break;
                        }
                    }
                }
            }
        }

        builder.Sort();
        return builder.ToImmutable();
    }

    private static string GenerateSource(FactoryInfo info, ImmutableArray<string> implementations)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using InversionOfControl.Model;");
        sb.AppendLine("using Microsoft.Extensions.Configuration;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Microsoft.Extensions.Logging;");
        sb.AppendLine();
        sb.AppendLine($"namespace {info.ConcreteFactoryNamespace};");
        sb.AppendLine();
        sb.AppendLine($"/// <summary>");
        sb.AppendLine($"/// Auto-generated bootstrap that registers <see cref=\"{info.ConcreteFactoryName}\"/> and its services.");
        sb.AppendLine($"/// </summary>");
        sb.AppendLine($"internal class {info.ConcreteFactoryName}Registration : IBootstrap");
        sb.AppendLine("{");
        sb.AppendLine("    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)");
        sb.AppendLine("    {");

        // Register each service implementation under its service interface
        foreach (var impl in implementations)
        {
            sb.AppendLine($"        services.Add{info.Lifetime}<{info.ServiceInterfaceFullName}, {impl}>();");
        }

        // Register the base EnumFactory<TEnum, TService> type
        sb.AppendLine($"        services.Add{info.Lifetime}<{info.BaseFactoryFullName}>();");

        // Register the concrete factory subclass if distinct from the base
        if (info.ConcreteFactoryFullName != info.BaseFactoryFullName)
        {
            sb.AppendLine($"        services.Add{info.Lifetime}<{info.ConcreteFactoryFullName}>();");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private readonly struct FactoryInfo : System.IEquatable<FactoryInfo>
    {
        public string ConcreteFactoryFullName { get; }
        public string ConcreteFactoryName { get; }
        public string ConcreteFactoryNamespace { get; }
        public string EnumTypeFullName { get; }
        public string ServiceInterfaceFullName { get; }
        public string ServiceInterfaceMetadataName { get; }
        public string BaseFactoryFullName { get; }
        public string Lifetime { get; }

        public FactoryInfo(
            string concreteFactoryFullName,
            string concreteFactoryName,
            string concreteFactoryNamespace,
            string enumTypeFullName,
            string serviceInterfaceFullName,
            string serviceInterfaceMetadataName,
            string baseFactoryFullName,
            string lifetime)
        {
            ConcreteFactoryFullName = concreteFactoryFullName;
            ConcreteFactoryName = concreteFactoryName;
            ConcreteFactoryNamespace = concreteFactoryNamespace;
            EnumTypeFullName = enumTypeFullName;
            ServiceInterfaceFullName = serviceInterfaceFullName;
            ServiceInterfaceMetadataName = serviceInterfaceMetadataName;
            BaseFactoryFullName = baseFactoryFullName;
            Lifetime = lifetime;
        }

        public bool Equals(FactoryInfo other)
        {
            return ConcreteFactoryFullName == other.ConcreteFactoryFullName;
        }

        public override bool Equals(object obj)
        {
            return obj is FactoryInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ConcreteFactoryFullName != null ? ConcreteFactoryFullName.GetHashCode() : 0;
        }
    }
}
