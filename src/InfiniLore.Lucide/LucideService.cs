// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Data;
using Microsoft.AspNetCore.Components;

namespace InfiniLore.Lucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class LucideService {
    private static readonly IDictionary<string, string>[] LookupSources = [
        LucideLookupDictionary.IconsByCamelCase,
        LucideLookupDictionary.IconsByPascalCase,
        LucideLookupDictionary.IconsByPascalLowerInvariant
    ];

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // ----------------------------------------------------------------------------------------------------------------
    public static MarkupString GetIconContent(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return new MarkupString(string.Empty);
        if (LucideLookupDictionary.IconsByLucideName.TryGetValue(iconName, out ILucideIconData? lucideIcon)) return new MarkupString(lucideIcon.SvgContent);

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int index = 0; index < LookupSources.Length; index++) {
            if (!LookupSources[index].TryGetValue(iconName, out string? lucideName)) continue;

            return new MarkupString(LucideLookupDictionary.IconsByLucideName[lucideName].SvgContent);
        }

        // Nothing was found
        return new MarkupString(string.Empty);
    }
}
