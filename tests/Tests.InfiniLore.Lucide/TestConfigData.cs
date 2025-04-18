// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;
using TUnit.Core.Interfaces;

namespace Tests.InfiniLore.Lucide;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TestConfigData {
    public int IconAmount { get; private init; }
    public Lazy<string[]> Icons { get; private init; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public TestConfigData() {
        IConfiguration config = TestContext.Configuration;
        if (!int.TryParse(config.Get("IconAmount") ?? "-1", out int totalIcons) || totalIcons == -1) {
            throw new Exception("IconAmount not found in test configuration");
        }

        IconAmount = totalIcons;
        Icons = new Lazy<string[]>(() => {
            string? iconNamesJson = config.Get("IconNames");
            if (string.IsNullOrEmpty(iconNamesJson)) {
                return Array.Empty<string>();
            }
            return JsonSerializer.Deserialize<string[]>(iconNamesJson) ?? Array.Empty<string>();
        });


    }
}
