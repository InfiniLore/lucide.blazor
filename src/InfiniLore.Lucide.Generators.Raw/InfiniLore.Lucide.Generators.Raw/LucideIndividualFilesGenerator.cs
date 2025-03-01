// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.GeneratorTools;
using InfiniLore.Lucide.Generators.Raw.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace InfiniLore.Lucide.Generators.Raw;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideIndividualFilesGenerator : IIncrementalGenerator {

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        context.RegisterSourceOutput(context.CollectLucideSvgFiles(), CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<LucideSvgFile> data) {
        var builder = new GeneratorStringBuilder();
        foreach ( LucideSvgFile lucideSvgFile in data) {
            string normalSvg = lucideSvgFile.Svg.TrimEnd();
            string noCommentSvg = Regex.Replace(normalSvg, "<!--.*?-->(\r\n|\r|\n)?", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
            string noWhitespaceSvg = Regex.Replace(normalSvg, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Multiline);
            string noWhitespaceAndNoCommentSvg = Regex.Replace(noCommentSvg, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Multiline);
            string svgContent = Regex.Match(normalSvg, @"<svg[^>]*>(.*?)</svg>", RegexOptions.Singleline | RegexOptions.Compiled)
                .Groups[1].Value
                .Split('\n')
                .Select(line => line.TrimStart())
                .Aggregate((a, b) => a + "\n" + b)
                .Trim();
            string svgContentFlat = Regex.Replace(svgContent, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Multiline);

            builder.WriteLucideLicense()
                .AppendLine()
                .AppendLine("// auto-generated")
                .AppendLine()
                .AppendLine("namespace InfiniLore.Lucide.Generators;")
                .AppendLine($"public class {lucideSvgFile.PascalCaseName} : ILucideIconData {{");

            builder.AppendLineIndented("public string DirectImport => _directImport;")
                .AppendLineIndented("private static readonly string _directImport = \"\"\"")
                .AppendLine(normalSvg)
                .AppendLine("\"\"\";")
                .AppendLine();

            
            builder
                .AppendLineIndented("public string DirectImportNoComments => _directImportNoComments;")
                .AppendLineIndented("private static readonly string _directImportNoComments = \"\"\"")
                .AppendLine(noCommentSvg)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string SvgContent => _svgContent;")
                .AppendLineIndented("private static readonly string _svgContent = \"\"\"")
                .AppendLine(svgContent)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string Flat => _flat;")
                .AppendLineIndented($"private static readonly string _flat = \"\"\"{noWhitespaceSvg}\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string FlatNoComments => _flatNoComments;")
                .AppendLineIndented($"private static readonly string _flatNoComments = \"\"\"{noWhitespaceAndNoCommentSvg}\"\"\";")
                .AppendLine();

            builder
                .AppendLineIndented("public string FlatSvgContent => _flatSvgContent;")
                .AppendLineIndented($"public static string _flatSvgContent => \"\"\"{svgContentFlat}\"\"\";")
                .AppendLine();

            builder.AppendLine("}")
                .AppendLine();

            context.AddSource($"{lucideSvgFile.PascalCaseName}.g.cs", builder.ToString());
            builder.Clear();
        }
    }
}
