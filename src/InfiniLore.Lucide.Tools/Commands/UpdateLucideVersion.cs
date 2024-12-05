// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CliArgsParser;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InfiniLore.Lucide.Tools.Commands;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UpdateLucideVersionCommands : ICommandAtlas {
    [Command("update-lucide-version")] [Description("Update lucide to the latest version")]
    public async Task CommandHello() {
        SuccessOrFailure<string, string> resultVersionNumber = await TryGetLatestVersionNumber();
        if (resultVersionNumber is {IsFailure: true, AsFailure: var failureString}) {
            Console.WriteLine(failureString);
            return;
        }
        Console.WriteLine($"Updated lucide to version {resultVersionNumber.AsSuccess.Value}");
        
        SuccessOrFailure<string, string> resultPackageJson = await TryUpdatePackageJson(resultVersionNumber.AsSuccess.Value);
        // TODO HERE
    }

    private async static Task<SuccessOrFailure<string, string>> TryGetLatestVersionNumber() {
        const string packageName = "lucide-static";
        const string npmRegistryUrl = $"https://registry.npmjs.org/{packageName}";

        using HttpClient client = new();
        try {
            HttpResponseMessage response = await client.GetAsync(npmRegistryUrl);
            response.EnsureSuccessStatusCode();

            // Parse the response
            string jsonResponse = await response.Content.ReadAsStringAsync();
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

            // Extract the latest version
            string? latestVersion = jsonDocument
                .RootElement
                .GetProperty("dist-tags")
                .GetProperty("latest")
                .GetString();

            if (latestVersion == null) {
                return new Failure<string>($"Could not find latest version of {packageName}");
            }
            return new Success<string>(latestVersion);
        }
        catch (Exception ex) {
            return new Failure<string>($"Error fetching package version: {ex.Message}");
        }
    }
    
    private async Task<SuccessOrFailure<string, string>> TryUpdatePackageJson(string latestVersion) {
        const string packageJsonPath = "../package.json";

        if (!File.Exists(packageJsonPath)) {
            return new Failure<string>("package.json file not found.");
        }
        try {
            // Read the existing package.json file
            string packageJsonContent = await File.ReadAllTextAsync(packageJsonPath);
            if (JsonNode.Parse(packageJsonContent) is not {} packageJson) return new Failure<string>("Failed to parse package.json.");

            // Navigate to the dependencies -> lucide-static property
            if (packageJson["dependencies"] is not {} dependencies) return new Failure<string>("Could not find 'dependencies' section in package.json.");
            if (dependencies["lucide-static"] is not {} currentVersionNode) return new Failure<string>("Could not find 'lucide-static' version in package.json.");

            string currentVersion = currentVersionNode.ToString().TrimStart('^');
            if (currentVersion == latestVersion) return new Failure<string>("The version in package.json is already up-to-date.");

            // Update the version
            dependencies["lucide-static"] = $"^{latestVersion}";

            // Write the updated JSON back to the package.json file
            string updatedPackageJsonContent = packageJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(packageJsonPath, updatedPackageJsonContent);

            return new Success<string>($"Updated lucide to version {latestVersion} in package.json.");
        } catch (Exception ex) {
            return new Failure<string>($"Error updating package.json: {ex.Message}");
        }
    }
}
