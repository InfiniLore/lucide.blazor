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
public record LucideNameDto(string Name) {
    public string PascalCaseName => Name.ToPascalCase();
    
    public static LucideNameDto FromAdditionalText(AdditionalText file, CancellationToken _ = default) 
        => new(Path.GetFileNameWithoutExtension(file.Path));
}
