// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using DevTools.InfiniLore.Lucide.Commands.UpdateLucide;
using DevTools.InfiniLore.Lucide.Library.Contracts;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DevTools.InfiniLore.Lucide.Library;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UpdateLucideStaticLibrary(IUpdateLucideParameters parameters, ILogger<UpdateLucideStaticLibrary> logger) {

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<string> TryGetLatestVersionNumber() {
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

            // ReSharper disable once InvertIf
            if (latestVersion == null) {
                logger.Error("Could not find latest version of {packageName}", packageName);
                return string.Empty;
            }

            return latestVersion;
        }
        catch (Exception ex) {
            logger.Error(ex, "Error fetching package version");
            return string.Empty;
        }
    }

    public async Task<bool> TryUpdatePackageJson(string latestVersion) {
        string packageJsonPath = parameters.AppendRoot("package.json");

        if (!File.Exists(packageJsonPath)) {
            logger.Error("package.json file not found.");
            return false;
        }

        try {
            // Read the existing package.json file
            string packageJsonContent = await File.ReadAllTextAsync(packageJsonPath);
            if (JsonNode.Parse(packageJsonContent) is not {} packageJson) {
                logger.Error("Could not parse package.json file.");
                return false;
            }

            // Navigate to the dependencies -> lucide-static property
            if (packageJson["dependencies"] is not {} dependencies) {
                logger.Error("Could not find 'dependencies' property in package.json.");
                return false;
            }

            if (dependencies["lucide-static"] is not {} currentVersionNode) {
                logger.Error("Could not find 'lucide-static' property in package.json.");
                return false;
            }

            string currentVersion = currentVersionNode.ToString().TrimStart('^');
            if (currentVersion == latestVersion) {
                logger.Information("Latest version of lucide-static is already installed.");
                return false;
            }

            // Update the version
            dependencies["lucide-static"] = latestVersion;

            // Write the updated JSON back to the package.json file
            string updatedPackageJsonContent = packageJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(packageJsonPath, updatedPackageJsonContent);

            logger.Information("Updated lucide-static version in package.json to {version}.", latestVersion);
            return true;
        }
        catch (Exception ex) {
            logger.Error(ex, "Error updating package.json");
            return false;
        }
    }

    public async Task<bool> TryRunNpmInstall(UpdateLucideStaticParameters args) {
        if (!Directory.Exists(args.Root)) {
            logger.Error("Working directory doesn't exist: {path}", args.Root);
            return false;
        }

        try {
            var npmProcess = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = args.NpmLocation,
                    Arguments = "install",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = args.Root
                }
            };

            npmProcess.Start();

            string output = await npmProcess.StandardOutput.ReadToEndAsync();
            string error = await npmProcess.StandardError.ReadToEndAsync();

            await npmProcess.WaitForExitAsync();

            if (npmProcess.ExitCode != 0) {
                logger.Error("npm install failed with error: {error}", error);
                return false;
            }
            logger.Information("npm install completed successfully with output: {output}", output);
            return true;

        }
        catch (Exception ex) {
            logger.Error(ex, "Error running npm install");
            return false;
        }
    }
}
