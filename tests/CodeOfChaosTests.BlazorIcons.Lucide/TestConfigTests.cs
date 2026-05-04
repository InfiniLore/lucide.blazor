// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaosTests.BlazorIcons.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[ClassDataSource<TestConfigData>(Shared = SharedType.PerTestSession)]
public class TestConfigTests(TestConfigData testConfig) {
    [Test]
    public async Task IsNotEmpty() {
        // Arrange & Act
        var iconAmount = testConfig.IconAmount;
        var iconNames = testConfig.Icons.Value;

        // Assert
        await Assert.That(iconAmount).IsNotEqualTo(0);
        await Assert.That(iconNames).IsNotEmpty();
        await Assert.That(iconNames).IsEqualTo(testConfig.Icons.Value);
    }
}
