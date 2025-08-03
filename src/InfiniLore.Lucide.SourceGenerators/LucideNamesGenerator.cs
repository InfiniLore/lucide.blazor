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
public class LucideNamesGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideNames(), CreateLucideNamesStore);
    }

    private static void CreateLucideNamesStore(SourceProductionContext context, ImmutableArray<LucideNameDto> data) {

        var builder = new GeneratorStringBuilder();

        var mem = new HashSet<string>();

        builder.WriteLucideLicense()
            .AppendLine()
            .AppendLine("// auto-generated")
            .AppendLine()
            .AppendLine("namespace InfiniLore.Lucide;")
            .AppendLine("public static class LucideNames {")
            .ForEachAppendLineIndented(data, dto => $"public const string {dto.PascalCaseName} = {dto.Name.ToQuotedString()};")
            .AppendLine()
            .Append("    public static string[] GetAsArray() => [")
                .ForEach(data, (b, dto) => {
                    if (mem.Add(dto.PascalCaseName)) { b.Append($"{dto.PascalCaseName},"); }
                })
                .Append("];")
            .AppendLine()
            .AppendLine("}");
        
        context.AddSource("LucideNames.g.cs", builder.ToString());
    }
}
#endif