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
public class LucideIndividualFilesGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideSvgFiles(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFileDto> data) {
        var builder = new GeneratorStringBuilder();
        foreach (LucideSvgFileDto lucideSvgFile in data) {
            builder.WriteLucideLicense()
                .AppendLine()
                .AppendLine("// auto-generated")
                .AppendLine()
                .AppendUsings("Microsoft.AspNetCore.Components")
                .AppendLine("namespace InfiniLore.Lucide.Data;")
                .AppendLine($"public class {lucideSvgFile.PascalCaseName} : ILucideIconData {{");

            builder.AppendLineIndented("public string DirectImport { get; } = \"\"\"")
                .AppendLine(lucideSvgFile.NormalSvg)
                .AppendLine("\"\"\";")
                .AppendLine();


            builder
                .AppendLineIndented("public string DirectImportNoComments { get; } = \"\"\"")
                .AppendLine(lucideSvgFile.NoCommentSvg)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string SvgContent { get; } = \"\"\"")
                .AppendLine(lucideSvgFile.SvgContent)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented($"public string Flat {{ get; }} = \"\"\"{lucideSvgFile.NoWhitespaceSvg}\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented($"public string FlatNoComments {{ get; }} = \"\"\"{lucideSvgFile.NoWhitespaceAndNoCommentSvg}\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented($"public string FlatSvgContent {{ get; }} = \"\"\"{lucideSvgFile.SvgContentFlat}\"\"\";")
                .AppendLine();

            builder.AppendLine("}")
                .AppendLine();

            context.AddSource($"{lucideSvgFile.PascalCaseName}.g.cs", builder.ToString());
            builder.Clear();
        }
    }
}
#endif
    