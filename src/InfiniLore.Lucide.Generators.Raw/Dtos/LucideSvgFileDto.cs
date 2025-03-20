// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Generators.Raw.Helpers;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Threading;

namespace InfiniLore.Lucide.Generators.Raw.Dtos;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record LucideSvgFileDto(string Name, string Svg) {
    public string PascalCaseName => Name.ToPascalCase();
    public string CamelCaseName => Name.ToCamelCase();
    
    public static LucideSvgFileDto FromAdditionalText(AdditionalText file, CancellationToken ct = default) 
        => new(
            Path.GetFileNameWithoutExtension(file.Path),
            file.GetText(ct)?.ToString() ?? string.Empty
        );
}
