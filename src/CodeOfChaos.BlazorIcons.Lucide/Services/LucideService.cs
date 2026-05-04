// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace CodeOfChaos.BlazorIcons.Lucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public class LucideService(ILucideDataProvider lookupDictionary) : ILucideService {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static string NormalizeIconName(ReadOnlySpan<char> iconName) {
        Span<char> buffer = stackalloc char[iconName.Length];
        return NormalizeIconNameValue(iconName, buffer).ToString();
    }
    
    public static ReadOnlySpan<char> NormalizeIconNameValue(in ReadOnlySpan<char> iconName, in Span<char> buffer) {
        int length = iconName.Length;
        int writeIndex = 0;
        for (int i = 0; i < length; i++) {
            char c = iconName[i];
            if (c is '-') continue;
            if (char.IsUpper(c)) buffer[writeIndex++] = char.ToLowerInvariant(c);
            else buffer[writeIndex++] = c;
        }

        return buffer[..writeIndex];
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
