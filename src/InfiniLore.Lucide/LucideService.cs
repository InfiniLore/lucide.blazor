// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Data;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace InfiniLore.Lucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public class LucideService(ILucideLookupDictionary lookupDictionary) : ILucideService {
    private static readonly MarkupString EmptyMarkupString = new(string.Empty);
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // ----------------------------------------------------------------------------------------------------------------
    public MarkupString GetIconContent(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return new MarkupString(string.Empty);
        
        string normalizedIconName = iconName.Replace("-", "").ToLowerInvariant();
        if (lookupDictionary.IconsByLucideName.TryGetValue(normalizedIconName, out Lazy<ILucideIconData>? lucideIcon))
            return new MarkupString(lucideIcon.Value.FlatSvgContent);
        
        // Nothing was found
        return EmptyMarkupString;
    }

    public MarkupString GetIconSvg(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return new MarkupString(string.Empty);

        string normalizedIconName = iconName.Replace("-", "").ToLowerInvariant();
        if (lookupDictionary.IconsByLucideName.TryGetValue(normalizedIconName, out Lazy<ILucideIconData>? lucideIcon))
            return new MarkupString(lucideIcon.Value.DirectImportNoComments);

        // Nothing was found
        return EmptyMarkupString;
    }

}
