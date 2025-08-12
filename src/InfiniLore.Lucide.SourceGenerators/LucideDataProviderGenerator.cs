#if NETSTANDARD2_0
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.SourceGenerators.Helpers;
using InfiniLore.Lucide.SourceGenerators.Dtos;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace InfiniLore.Lucide.SourceGenerators;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideDataProviderGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideIcons(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFileDto> data) {
        var builder = new GeneratorStringBuilder();

        var mem = new HashSet<string>();
        
        builder
            .AppendUsings(
                "System",
                "System.Collections.Concurrent",
                "System.Collections.Generic"
            )
            .AppendLine("namespace InfiniLore.Lucide;")
            .AppendLine("public partial class LucideDataProvider {")
            .Indent(b => b
                .AppendLine("private readonly ConcurrentDictionary<string, string> _cache = new();")
                
                .AppendLine("public string GetIconSvgData(string iconName) {")
                .Indent(b1 => b1
                    .AppendLine("if (_cache.TryGetValue(iconName, out var svg)) return svg;")
                    .AppendLine("string data = iconName switch {")
                    .ForEachAppendLineIndented(
                        data,
                        dto => {
                            if (!mem.Add(dto.NormalizedName)) return string.Empty;
                            return $"\"{dto.NormalizedName}\" => \"\"\"{dto.SvgContent}\"\"\",";
                        })
                    .AppendLineIndented("_ => string.Empty")
                    .AppendLine("};")
                    .AppendLine("if (!string.IsNullOrEmpty(data)) _cache.AddOrUpdate(iconName, data, (_, __) => data);")
                    .AppendLine("return data;")
                )
                .AppendLine("}")
            )
            .Indent(b => b
                .AppendLine($"public int Count => {data.Length};")
            )
            .AppendLine("}");

        context.AddSource("LucideDataProvider.g.cs", builder.ToString());
    }
}
#endif