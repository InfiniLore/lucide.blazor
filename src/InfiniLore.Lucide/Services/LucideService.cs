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
    private static string NormalizeIconName(ReadOnlySpan<char> iconName) {
        int length = iconName.Length;
        int writeIndex = 0;
        Span<char> buffer = stackalloc char[iconName.Length];
        for (int i = 0; i < length; i++) {
            char c = iconName[i];
            if (c is '-') continue;
            if (char.IsUpper(c)) buffer[writeIndex++] = char.ToLowerInvariant(c);
            else buffer[writeIndex++] = c;
        }
        return buffer[..writeIndex].ToString();
    }

    public MarkupString GetIconAsMarkupString(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return new MarkupString(string.Empty);
        
        return new MarkupString( lookupDictionary.GetIconSvgData(NormalizeIconName(iconName)));
    }
    
    public string GetIconAsString(string iconName) {
        if (string.IsNullOrWhiteSpace(iconName)) return string.Empty;
        
        return lookupDictionary.GetIconSvgData(NormalizeIconName(iconName));
    }
}
