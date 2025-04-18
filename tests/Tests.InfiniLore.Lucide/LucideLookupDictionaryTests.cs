// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Data;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ClassDataSource<TestConfigData>(Shared = SharedType.PerTestSession)]
public class LucideLookupDictionaryTests(TestConfigData testConfig) {
    [Test]
    public async Task IsNotEmpty() {
        // Arrange
        var lucideLookupDictionary = new LucideLookupDictionary();
        
        // Act
        int count = lucideLookupDictionary.Count;

        // Assert
        await Assert.That(count).IsNotZero()
            .And.IsEqualTo(testConfig.TotalIcons); 
    }
}
