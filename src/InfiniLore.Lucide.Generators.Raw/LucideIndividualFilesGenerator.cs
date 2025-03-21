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

            builder.AppendLineIndented("public string DirectImport => _directImport;")
                .AppendLineIndented("private static readonly string _directImport = \"\"\"")
                .AppendLine(lucideSvgFile.NormalSvg)
                .AppendLine("\"\"\";")
                .AppendLine();


            builder
                .AppendLineIndented("public string DirectImportNoComments => _directImportNoComments;")
                .AppendLineIndented("private static readonly string _directImportNoComments = \"\"\"")
                .AppendLine(lucideSvgFile.NoCommentSvg)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string SvgContent => _svgContent;")
                .AppendLineIndented("private static readonly string _svgContent = \"\"\"")
                .AppendLine(lucideSvgFile.SvgContent)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string Flat => _flat;")
                .AppendLineIndented($"private static readonly string _flat = \"\"\"{lucideSvgFile.NoWhitespaceSvg}\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string FlatNoComments => _flatNoComments;")
                .AppendLineIndented($"private static readonly string _flatNoComments = \"\"\"{lucideSvgFile.NoWhitespaceAndNoCommentSvg}\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string FlatSvgContent => _flatSvgContent;")
                .AppendLineIndented($"public static readonly string _flatSvgContent = \"\"\"{lucideSvgFile.SvgContentFlat}\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented($"public static readonly MarkupString FlatMarkup = new(\"\"\"{lucideSvgFile.SvgContentFlat}\"\"\");")
                .AppendLine();

            builder.AppendLine("}")
                .AppendLine();

            context.AddSource($"{lucideSvgFile.PascalCaseName}.g.cs", builder.ToString());
            builder.Clear();
        }
    }
}
#endif
    