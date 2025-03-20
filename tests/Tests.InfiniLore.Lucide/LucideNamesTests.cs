// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Lucide.Data;
using System.Reflection;
using Type=System.Type;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LucideNamesTests {
    [Test]
    public async Task IsMappedCorrectly() {
        // Arrange
        
        // Act &  Assert
        #pragma warning disable TUnitAssertions0005
        await Assert.That(LucideNames.Signature).IsEqualTo("signature");
        #pragma warning restore TUnitAssertions0005
    }

    [Test]
    public async Task IsNotEmpty() {
        // Arrange
        Type type = typeof(LucideNames);
        List<FieldInfo> constStringFields = type
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .ToList();
        
        // Act
        int count = constStringFields.Count;

        // Assert
        await Assert.That(count).IsNotZero()
            .And.IsGreaterThanOrEqualTo(1560); // Yes Lucide has more than 1500 icons, but we don't want to test them all at the moment
    }
}
