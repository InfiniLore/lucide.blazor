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
public class LucideNamesGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideNames(), CreateLucideNamesStore);
    }

    private static void CreateLucideNamesStore(SourceProductionContext context, ImmutableArray<LucideNameDto> data) {
        
        var builder = new GeneratorStringBuilder();
        
        builder.WriteLucideLicense()
            .AppendLine()
            .AppendLine("// auto-generated")
            .AppendLine()
            .AppendLine("namespace InfiniLore.Lucide.Data;")
            .AppendLine("public static class LucideNames {")
            .ForEachAppendLineIndented(data, dto => $"public const string {dto.PascalCaseName} = {dto.Name.ToQuotedString()};")
            .AppendLine("}");
        
        context.AddSource("LucideNames.g.cs", builder.ToString());
    }
}
#endif