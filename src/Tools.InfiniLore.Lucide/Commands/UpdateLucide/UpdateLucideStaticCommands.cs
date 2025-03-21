// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CodeOfChaos.CliArgsParser;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tools.InfiniLore.Lucide.Commands.GenerateRazor;
using Tools.InfiniLore.Lucide.Setup;

namespace Tools.InfiniLore.Lucide.Commands.UpdateLucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
[CliArgsCommand("update-lucide-static")]
public partial class UpdateLucideStaticCommands : ICommand<UpdateLucideStaticParameters> {
    private IServiceProvider Provider { get; } = ServiceProviderFactory.CreateProvider();
    private ILogger<GenerateRazorCommand>? _loggerCache;
    private ILogger<GenerateRazorCommand> Logger => _loggerCache ??= Provider.GetRequiredService<ILogger<GenerateRazorCommand>>();
    
    
    public async Task ExecuteAsync(UpdateLucideStaticParameters args) {
        await GlobalCatcher.ExecuteWithGlobalExceptionHandlingAsync(async () => {
            SuccessOrFailure<string, string> resultVersionNumber = await TryGetLatestVersionNumber();
            if (resultVersionNumber is { IsFailure: true, AsFailure.Value: var failureString }) {
                Logger.Error(failureString);
                throw new Exception(failureString);
            }

            Logger.Information("Lucide Latest version is {version}", resultVersionNumber.AsSuccess.Value);

            SuccessOrFailure<string, string> resultPackageJson = await TryUpdatePackageJson(resultVersionNumber.AsSuccess.Value, args);
            if (resultPackageJson is { IsFailure: true, AsFailure.Value: var failureString2 }) {
                Logger.Error(failureString2);
                throw new Exception(failureString2);
            }

            Logger.Information(resultPackageJson.AsSuccess.Value);

            SuccessOrFailure<string, string> resultNpmInstall = await RunNpmInstall(args);
            if (resultNpmInstall is { IsFailure: true, AsFailure.Value: var failureString3 }) {
                Logger.Error(failureString3);
                throw new Exception(failureString3);
            }

            Logger.Information(resultNpmInstall.AsSuccess.Value);
        });
    }

    private static async Task<SuccessOrFailure<string, string>> TryGetLatestVersionNumber() {
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

    private static async Task<SuccessOrFailure<string, string>> TryUpdatePackageJson(string latestVersion, UpdateLucideStaticParameters args) {
        string packageJsonPath = args.AppendRoot("package.json");

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
            dependencies["lucide-static"] = latestVersion;

            // Write the updated JSON back to the package.json file
            string updatedPackageJsonContent = packageJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(packageJsonPath, updatedPackageJsonContent);

            return new Success<string>($"Updated lucide to version {latestVersion} in package.json.");
        }
        catch (Exception ex) {
            return new Failure<string>($"Error updating package.json: {ex.Message}");
        }
    }

    private static async Task<SuccessOrFailure<string, string>> RunNpmInstall(UpdateLucideStaticParameters args) {
        if (!Directory.Exists(args.Root)) {
            return new Failure<string>("Working directory doesn't exist: " + args.Root);
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

            return npmProcess.ExitCode == 0
                ? new Success<string>($"npm install completed successfully with output: {output}")
                : new Failure<string>($"npm install failed with error: {error}");

        }
        catch (Exception ex) {
            return new Failure<string>($"Exception during npm install: {ex.Message}");
        }
    }
}
