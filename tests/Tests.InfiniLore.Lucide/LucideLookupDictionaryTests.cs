// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Data;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LucideLookupDictionaryTests {
    [Test]
    public async Task IsNotEmpty() {
        // Arrange
        var lucideLookupDictionary = new LucideLookupDictionary();
        int expected = int.Parse(TestContext.Configuration.Get("TotalIcons") ?? "-1");
        if (expected == -1) throw new Exception("TotalIcons not found in test configuration");
        
        // Act
        int count = lucideLookupDictionary.Count;

        // Assert
        await Assert.That(count).IsNotZero()
            .And.IsGreaterThanOrEqualTo(expected); // Yes Lucide has more than 1500 icons, but we don't want to test them all at the moment
    }
}
