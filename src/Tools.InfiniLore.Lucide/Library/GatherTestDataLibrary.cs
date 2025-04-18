// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Playwright;
using Serilog;
using System.Text.Json;
using System.Text.RegularExpressions;
using Tools.InfiniLore.Lucide.Library.Contracts;

namespace Tools.InfiniLore.Lucide.Library;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class GatherTestDataLibrary(IUpdateLucideParameters parameters, ILogger logger) {
    [GeneratedRegex(@"Search (\d+) icons")]
    private static partial Regex ExtractRegex { get; }

    private static readonly JsonSerializerOptions Options = new() {
        WriteIndented = true
    };

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task SaveDataToTestConfigAsync(TestData data) {
        string filePath = Path.Combine(parameters.Root, "tests/Tests.InfiniLore.Lucide/testconfig.json");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        try {
            await File.WriteAllTextAsync(
                filePath,
                JsonSerializer.Serialize(data, Options)
            );
        }

        catch (Exception e) {
            logger.Error(e, "Error writing testconfig.json");
            throw;
        }

    }

    public async Task<int> GatherIconAmountAsync() {
        using IPlaywright? playwright = await Playwright.CreateAsync();
        var options = new BrowserTypeLaunchOptions {
            Headless = true,
            SlowMo = 50,
            Timeout = 30000
        };

        await using IBrowser browser = await playwright.Chromium.LaunchAsync(options);
        IPage page = await browser.NewPageAsync();

        await page.GotoAsync("https://lucide.dev");

        // Wait for the search button to be visible and get its text
        IElementHandle? searchButton = await page.WaitForSelectorAsync(
            "#VPContent > div > div.VPHero.has-image.VPHomeHero > div > div.image > div > div.card-wrapper > div > button"
        );

        if (searchButton is null) {
            logger.Error("Could not find search button");
            return -1;
        }

        string? buttonText = await searchButton.TextContentAsync();

        // Extract the number using regex
        if (buttonText == null) {
            logger.Error("Could not find search button text");
            return -1;
        }

        Match match = ExtractRegex.Match(buttonText);
        // ReSharper disable once InvertIf
        if (!match.Success) {
            logger.Error("Could not extract number from search button text");
            return -1;
        }

        return int.Parse(match.Groups[1].Value);
    }
}
