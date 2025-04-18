// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TestConfigData {
    public int TotalIcons { get; private set; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public TestConfigData() {
        // ReSharper disable once DuplicatedSequentialIfBodies
        if(!int.TryParse(TestContext.Configuration.Get("TotalIcons") ?? "-1", out int totalIcons)) throw new Exception("TotalIcons not found in test configuration");
        if(totalIcons == -1) throw new Exception("TotalIcons not found in test configuration");

        TotalIcons = totalIcons;
    }
}
