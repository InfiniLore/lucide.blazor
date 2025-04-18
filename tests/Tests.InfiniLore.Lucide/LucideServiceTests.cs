// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide;
using InfiniLore.Lucide.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ClassDataSource<TestConfigData>(Shared = SharedType.PerTestSession)]
public class LucideServiceTests(TestConfigData testConfig) {
    [Test]
    public async Task AddedServicesToServiceCollection() {
        // Arrange
        var serviceCollection = new ServiceCollection();
        
        // Act
        serviceCollection.AddLucideIcons();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        // Assert
        await Assert.That(serviceProvider.GetService<ILucideLookupDictionary>()).IsNotNull();
        await Assert.That(serviceProvider.GetService<ILucideService>()).IsNotNull();
    }
    
    [Test]
    [Arguments("signature")]
    public async Task CanFindSvgContent(string iconName) {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLucideIcons();
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        var lucideService = serviceProvider.GetRequiredService<ILucideService>();

        // Act
        MarkupString markup = lucideService.GetIconContent(iconName);
        string data = markup.Value;
        
        // Assert
        await Assert.That(data).IsNotNullOrWhitespace();
    }
    
    [Test]
    public async Task ContainsAllIcons() {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLucideIcons();
        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        var lucideService = serviceProvider.GetRequiredService<ILucideService>();
        
        // Act
        await Assert.That(testConfig.Icons.Value).IsNotEmpty();
        await Parallel.ForEachAsync(testConfig.Icons.Value, async (iconName, _) => {
            // Needed because we use this lookup in a normalized way, so we can have a broader input
            string iconNameNormalized = iconName.Replace("-", "").ToLowerInvariant();
            
            MarkupString markup = lucideService.GetIconContent(iconNameNormalized);
            string data = markup.Value;
            
            await Assert.That(data).IsNotNullOrWhitespace();
        } );
    }
}
