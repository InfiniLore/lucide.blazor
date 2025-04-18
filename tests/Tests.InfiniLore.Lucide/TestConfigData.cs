// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TestConfigData {
    public int TotalIcons { get; private init; }
    public Lazy<string[]> Icons { get; private init; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public TestConfigData() {
        // ReSharper disable once DuplicatedSequentialIfBodies
        if(!int.TryParse(TestContext.Configuration.Get("IconAmount") ?? "-1", out int totalIcons)) throw new Exception("TotalIcons not found in test configuration");
        if(totalIcons == -1) throw new Exception("TotalIcons not found in test configuration");

        TotalIcons = totalIcons;
        Icons = new Lazy<string[]>(() => TestContext.Configuration.Get("Icons")?.Split(';') ?? Array.Empty<string>());
    }
}
