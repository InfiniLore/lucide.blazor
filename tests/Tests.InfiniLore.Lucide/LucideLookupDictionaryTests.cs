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
        
        // Act
        int count = LucideLookupDictionary.Count;

        // Assert
        await Assert.That(count).IsNotZero()
            .And.IsGreaterThanOrEqualTo(1560); // Yes Lucide has more than 1500 icons, but we don't want to test them all at the moment
    }
}
