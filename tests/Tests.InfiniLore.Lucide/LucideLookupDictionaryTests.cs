// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide;
using InfiniLore.Lucide.Data;
using System.Collections.Frozen;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ClassDataSource<TestConfigData>(Shared = SharedType.PerTestSession)]
public class LucideLookupDictionaryTests(TestConfigData testConfig) {
    [Test]
    public async Task IsNotEmpty() {
        // Arrange
        var lucideLookupDictionary = new LucideDataProvider();
        
        // Act
        int count = lucideLookupDictionary.Count;

        // Assert
        await Assert.That(count).IsNotZero()
            .And.IsEqualTo(testConfig.IconAmount)
            .And.IsEqualTo(testConfig.Icons.Value.Length); 
    }
    
    [Test]
    public async Task ContainsAllIcons() {
        // Arrange
        var lucideLookupDictionary = new LucideDataProvider();
        FrozenDictionary<string, Lazy<ILucideIconData>> lookup = lucideLookupDictionary.IconsByLucideName;
        
        // Act
        await Assert.That(testConfig.Icons.Value).IsNotEmpty();
        await Parallel.ForEachAsync(testConfig.Icons.Value, async (iconName, _) => {
            // Needed because we use this lookup in a normalized way, so we can have a broader input
            string iconNameNormalized = iconName.Replace("-", "").ToLowerInvariant();
            
            await Assert.That(lookup).ContainsKey(iconNameNormalized);
            await Assert.That(lookup[iconNameNormalized].Value.Content).IsNotNullOrWhitespace();
        } );
    }
}
