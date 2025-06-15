#if NETSTANDARD2_0
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.SourceGenerators.Helpers;
using InfiniLore.Lucide.SourceGenerators.Dtos;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace InfiniLore.Lucide.SourceGenerators;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideDataProviderGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideSvgFiles(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFileDto> data) {
        var builder = new GeneratorStringBuilder();

        builder
            .AppendUsings(
                "System",
                "System.Collections.Frozen",
                "System.Collections.Generic",
                "InfiniLore.Lucide.Data"
            )
            .AppendLine("namespace InfiniLore.Lucide;")
            .AppendLine("public partial class LucideDataProvider {")
            .Indent(b => { b
                .AppendLine("public FrozenDictionary<string, Lazy<string>> IconSvgData { get; } = new Dictionary<string, Lazy<ILucideIconData>>() {")
                .ForEachAppendLineIndented(
                    data,
                    dto => $"[\"{dto.NormalizedName}\"] = new Lazy<string>(static () => \"\"\"{dto.SvgContentFlat}\"\"\"),"
                )
                .AppendLine("}.ToFrozenDictionary();");
            })
            .AppendLine("}");

        context.AddSource("LucideDataProvider.g.cs", builder.ToString());
    }
}
#endif