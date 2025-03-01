// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide;
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

        // Act
        MarkupString markup = LucideService.GetIconContent(iconName);
        string data = markup.Value;
        
        // Assert
        await Assert.That(data).IsNotNullOrWhitespace();
    }
}
