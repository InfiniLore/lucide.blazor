// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Generators.Raw.Helpers;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace InfiniLore.Lucide.Generators.Raw.Dtos;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record LucideSvgFileDto(string Name, string Svg) {
    public string PascalCaseName => Name.ToPascalCase();
    public string CamelCaseName => Name.ToCamelCase();
    
    public string NormalSvg => Svg.TrimEnd();
    public string NoCommentSvg => Regex.Replace(NormalSvg, "<!--.*?-->(\r\n|\r|\n)?", string.Empty, RegexOptions.Compiled | RegexOptions.Multiline);
    public string NoWhitespaceSvg => Regex.Replace(NormalSvg, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Multiline);
    public string NoWhitespaceAndNoCommentSvg => Regex.Replace(NoCommentSvg, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Multiline);
    public string SvgContent => Regex.Match(NormalSvg, @"<svg[^>]*>(.*?)</svg>", RegexOptions.Singleline | RegexOptions.Compiled)
        .Groups[1].Value
        .Split('\n')
        .Select(line => line.TrimStart())
        .Aggregate((a, b) => a + "\n" + b)
        .Trim();
    public string SvgContentFlat => Regex.Replace(SvgContent, @"\s+", " ", RegexOptions.Compiled | RegexOptions.Multiline);
    
    public static LucideSvgFileDto FromAdditionalText(AdditionalText file, CancellationToken ct = default) 
        => new(
            Path.GetFileNameWithoutExtension(file.Path),
            file.GetText(ct)?.ToString() ?? string.Empty
        );
}
