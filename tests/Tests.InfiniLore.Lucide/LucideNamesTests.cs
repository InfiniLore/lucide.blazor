// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Extensions;
using InfiniLore.Lucide;
using System.Collections.Frozen;
using System.Reflection;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ClassDataSource<TestConfigData>(Shared = SharedType.PerTestSession)]
public class LucideNamesTests(TestConfigData testConfig) {
    [Test]
    public async Task IsMappedCorrectly() {
        // Arrange
        
        // Act & Assert
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
            .And.IsEqualTo(testConfig.IconAmount);
    }
    
    [Test]
    public async Task ContainsAllIcons() {
        // Arrange
        Type type = typeof(LucideNames);
        List<(string FieldName, string? lucideName)> data = type
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (f.Name, f.GetRawConstantValue() as string))
            .ToList();
        
        FrozenSet<string> iconNames = testConfig.Icons.Value.ToFrozenSet();
        
        // Act
        await Assert.That(data).HasCount().EqualTo(testConfig.IconAmount);
        await Parallel.ForEachAsync(data, async (tuple, _) => {
            await Assert.That(tuple.lucideName).IsNotNullOrWhitespace();
            
            // Needed because we use this lookup in a normalized way, so we can have a broader input
            string iconNameField = tuple.lucideName!.ToPascalCase();
            
            await Assert.That(iconNames).Contains(tuple.lucideName);
            await Assert.That(tuple.FieldName.ToLowerInvariant()).IsEqualTo(iconNameField.ToLowerInvariant()); // fixes a small issue where the 
        });
    }
}
