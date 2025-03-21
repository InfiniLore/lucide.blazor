#if NETSTANDARD2_0
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.Generators.Raw.Dtos;
using InfiniLore.Lucide.Generators.Raw.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace InfiniLore.Lucide.Generators.Raw;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideLookupDictionaryGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideSvgFiles(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFileDto> data) {
        var builder = new GeneratorStringBuilder();

        builder
            .AppendUsings(
                "System",
                "System.Collections.Frozen",
                "System.Collections.Generic"
            )
            .AppendLine("namespace InfiniLore.Lucide.Data;")
            .AppendLine("public partial class LucideLookupDictionary {");

        builder.Indent(b => {
            b.AppendLine("public FrozenDictionary<string, Lazy<ILucideIconData>> IconsByLucideName { get; } = new Dictionary<string, Lazy<ILucideIconData>>() {");
            b.ForEachAppendLineIndented(data, itemFormatter: d => $"[\"{d.NormalizedName}\"] = new Lazy<ILucideIconData>(static () => new {d.PascalCaseName}()),");
            b.AppendLine("}.ToFrozenDictionary();");
        });

        builder.AppendLine("}");

        context.AddSource("LucideLookupDictionary.g.cs", builder.ToString());
    }
}
#endif