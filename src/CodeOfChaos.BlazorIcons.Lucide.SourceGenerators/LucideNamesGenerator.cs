#if NETSTANDARD2_0
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.BlazorIcons.Lucide.SourceGenerators.Dtos;
using CodeOfChaos.GeneratorTools;
using CodeOfChaos.BlazorIcons.Lucide.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CodeOfChaos.BlazorIcons.Lucide.SourceGenerators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideNamesGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectManyLucideIcons(), CreateLucideNamesStore);
    }

    private static void CreateLucideNamesStore(SourceProductionContext context, ImmutableArray<LucideNameDto> data) {

        var builder = new GeneratorStringBuilder();

        var mem = new HashSet<string>();

        builder.WriteLucideLicense()
            .AppendLine()
            .AppendLine("// auto-generated")
            .AppendLine()
            .AppendLine("namespace CodeOfChaos.BlazorIcons.Lucide;")
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