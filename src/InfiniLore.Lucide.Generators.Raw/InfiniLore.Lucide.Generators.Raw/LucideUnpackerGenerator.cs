// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Generators.Raw.Helpers;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InfiniLore.Lucide.Generators.Raw;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[Generator(LanguageNames.CSharp)]
public class LucideUnpackerGenerator : IIncrementalGenerator {
    private const string RegexIconSvgPath = @"lucide-static\\icons\\.*\.svg$";

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        IncrementalValuesProvider<AdditionalText> files = context.AdditionalTextsProvider
            .Where(file => Regex.IsMatch(file.Path, RegexIconSvgPath));

        IncrementalValueProvider<ImmutableArray<(string Name, string Svg)>> iconsProvider = files
            .Select((file, cancellationToken) => (
                    Name: Path.GetFileNameWithoutExtension(file.Path),
                    Svg: file.GetText(cancellationToken)?.ToString() ?? string.Empty
                ))
            .Collect();

        context.RegisterSourceOutput(iconsProvider, CreateIconFiles);
    }

    private static void CreateIconFiles(SourceProductionContext context, ImmutableArray<(string Name, string Svg)> data) {
        var builder = new StringBuilder();
        foreach ((string name, string svg) in data) {
            string normalSvg = svg.TrimEnd();
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
            string pascalName = name.ToPascalCase();

            builder.WriteLucideLicense()
                .AppendLine()
                .AppendLine("// auto-generated")
                .AppendLine()
                .AppendLine("namespace InfiniLore.Lucide.Raw;")
                .AppendLine($"public class {pascalName} {{");

            builder.IndentLine(1, "public static string DirectImport => \"\"\"")
                .AppendLine(normalSvg)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder.IndentLine(1, "public static string DirectImportNoComments => \"\"\"")
                .AppendLine(noCommentSvg)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder.IndentLine(1, "public static string SvgContent => \"\"\"")
                .AppendLine(svgContent)
                .AppendLine("\"\"\";")
                .AppendLine();

            builder.IndentLine(1, $"public static string Flat => \"\"\"{noWhitespaceSvg}\"\"\";")
                .AppendLine();

            builder.IndentLine(1, $"public static string FlatNoComments => \"\"\"{noWhitespaceAndNoCommentSvg}\"\"\";")
                .AppendLine();

            builder.IndentLine(1, $"public static string FlatSvgContent => \"\"\"{svgContentFlat}\"\"\";")
                .AppendLine();

            builder.AppendLine("}")
                .AppendLine();

            context.AddSource($"{pascalName}.g.cs", builder.ToString());
            builder.Clear();
        }
    }
}
