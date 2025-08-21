// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.SourceGenerators.Dtos;
using InfiniLore.Lucide.SourceGenerators.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace InfiniLore.Lucide.SourceGenerators;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideDynamicIconGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideIcons(), CreateIconFiles);
    }
    
    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFileDto> data) {
        var builder = new GeneratorStringBuilder();
        
        builder
            .AppendUsings("System.Diagnostics.CodeAnalysis")
            .AppendNamespace("InfiniLore.Lucide")
            .AppendLine("#nullable")
            .AppendLine("public partial class LucideDynamicIcon {")
            .AppendLineIndented("public static bool TryGetComponentType(ReadOnlySpan<char> name, [NotNullWhen(true)] out Type? type) {")
            .Indent(b => b
                .AppendLineIndented("Span<char> buffer = stackalloc char[name.Length];")
                .AppendLineIndented("var normalized = LucideService.NormalizeIconNameValue(name, buffer);")
                .AppendLineIndented("type = normalized switch {")
                .ForEachAppendLineIndented(data, dto => $"\"{dto.NormalizedName}\" => typeof(Li{dto.PascalCaseName}),")
                .AppendLineIndented("_ => null")
                .AppendLineIndented("};")
                .AppendLineIndented("return type is not null;")
            )
            .AppendLineIndented("}")
            .AppendLine("}");
        
        context.AddSource("LucideDynamicIcon.g.cs", builder.ToString());
    }
}
