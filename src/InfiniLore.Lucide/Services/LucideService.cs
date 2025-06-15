// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace InfiniLore.Lucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public class LucideService(ILucideDataProvider lookupDictionary) : ILucideService {
    private static readonly MarkupString EmptyMarkupString = new(string.Empty);
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // ----------------------------------------------------------------------------------------------------------------
    public MarkupString GetIconContent(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return new MarkupString(string.Empty);
        
        string normalizedIconName = iconName.Replace("-", "").ToLowerInvariant();
        
        return lookupDictionary.IconsByLucideName.TryGetValue(normalizedIconName, out Lazy<ILucideIconData>? lucideIcon)
            ? new MarkupString(lucideIcon.Value.Content) 
            : EmptyMarkupString;
    }
}
