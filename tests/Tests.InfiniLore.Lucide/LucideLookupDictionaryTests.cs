// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide;

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
        await Assert.That(count).IsNotEqualTo(0)
            .And.IsEqualTo(testConfig.IconAmount)
            .And.IsEqualTo(testConfig.Icons.Value.Length); 
    }
    
    [Test]
    public async Task ContainsAllIcons() {
        // Arrange
        var lucideLookupDictionary = new LucideDataProvider();
        string[]? allNames = LucideNames.GetAsArray();
        List<string> allNamesNormalized = allNames.Select(n => n.Replace("-", "").ToLowerInvariant()).ToList();
        var allNamesSet = new HashSet<string>(allNamesNormalized);
        int allNamesCount = allNames.Length;
        
        // Act
        await Assert.That(lucideLookupDictionary.Count).IsNotEqualTo(0)
            .And.IsEqualTo(allNamesCount);
        
        await Assert.That(testConfig.Icons.Value).IsNotEmpty();
        
        await Parallel.ForEachAsync(testConfig.Icons.Value, async (iconName, _) => {
            // Needed because we use this lookup in a normalized way, so we can have a broader input
            string iconNameNormalized = iconName.Replace("-", "").ToLowerInvariant();
            
            await Assert.That(allNamesSet).Contains(iconNameNormalized);
            string svgContent = lucideLookupDictionary.GetIconSvgData(iconNameNormalized);
            await Assert.That(svgContent).IsNotNullOrWhiteSpace();
        } );
    }
}
