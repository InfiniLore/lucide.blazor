// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide;
using InfiniLore.Lucide.Data;
using Microsoft.AspNetCore.Components;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LucideServiceTests {
    [Test]
    [Arguments("signature")]
    public async Task CanFindSvgContent(string iconName) {
        // Arrange
        var lucideLookupDictionary = new LucideLookupDictionary();
        var lucideService = new LucideService(lucideLookupDictionary);

        // Act
        MarkupString markup = lucideService.GetIconContent(iconName);
        string data = markup.Value;
        
        // Assert
        await Assert.That(data).IsNotNullOrWhitespace();
    }
}
