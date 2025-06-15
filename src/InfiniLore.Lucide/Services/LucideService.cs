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
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static string NormalizeIconName(string iconName) => iconName.Replace("-", "").ToLowerInvariant();
    
    public MarkupString GetIconAsMarkupString(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return new MarkupString(string.Empty);
        
        return new MarkupString( lookupDictionary.GetIconSvgData(NormalizeIconName(iconName)));
    }
    
    public string GetIconAsString(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return string.Empty;
        
        return lookupDictionary.GetIconSvgData(NormalizeIconName(iconName));
    }
}
