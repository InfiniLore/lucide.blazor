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
public class LucideIndividualFilesGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideSvgFiles(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFileDto> data) {
        var builder = new GeneratorStringBuilder();
        foreach (LucideSvgFileDto lucideSvgFile in data) {
            builder.WriteLucideLicense()
                .AppendLine("// auto-generated")
                .AppendLine("namespace InfiniLore.Lucide.Data;")
                .AppendLine($"public class {lucideSvgFile.PascalCaseName} : ILucideIconData {{")
                .AppendLineIndented($"private const string SvgContent = \"\"\"{lucideSvgFile.SvgContentFlat}\"\"\";")
                .AppendLineIndented("public string Content => SvgContent;")
                .AppendLine("}");

            context.AddSource($"{lucideSvgFile.PascalCaseName}.g.cs", builder.ToString());
            builder.Clear();
        }
    }
}
#endif
    