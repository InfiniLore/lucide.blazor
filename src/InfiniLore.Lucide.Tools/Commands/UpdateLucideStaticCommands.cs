// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CliArgsParser;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InfiniLore.Lucide.Tools.Commands;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     The <c>UpdateLucideStaticCommands</c> class provides a command to update the Lucide library to its latest version.
/// </summary>
/// <remarks>
///     This class implements the <see cref="ICommandAtlas" /> interface and defines the command "update-lucide-static".
///     It facilitates fetching the latest version number of Lucide, updating the package.json file, and running npm
///     install
///     in the specified project directory.
/// </remarks>
public class UpdateLucideStaticCommands : ICommandAtlas {
    /// <summary>
    ///     Executes the update command for Lucide to the latest version.
    /// </summary>
    /// <param name="args">Parameters required to perform the update, including the root directory and npm location.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Command<UpdateLucideStaticParameters>("update-lucide-static")] [Description("Update lucide to the latest version")]
    public async Task CommandHello(UpdateLucideStaticParameters args) {
        SuccessOrFailure<string, string> resultVersionNumber = await TryGetLatestVersionNumber();
        if (resultVersionNumber is { IsFailure: true, AsFailure.Value: var failureString }) {
            Console.WriteLine(failureString);
            return;
        }
        Console.WriteLine($"Lucide Latest version is {resultVersionNumber.AsSuccess.Value}");

        SuccessOrFailure<string, string> resultPackageJson = await TryUpdatePackageJson(resultVersionNumber.AsSuccess.Value, args);
        if (resultPackageJson is { IsFailure: true, AsFailure.Value: var failureString2 }) {
            Console.WriteLine(failureString2);
            // return; // We don't want to stop the update if the package.json update fails.
        }
        Console.WriteLine(resultPackageJson.AsSuccess.Value);

        SuccessOrFailure<string, string> resultNpmInstall = await RunNpmInstall(args);
        if (resultNpmInstall is { IsFailure: true, AsFailure.Value: var failureString3 }) {
            Console.WriteLine(failureString3);
            return;
        }
        Console.WriteLine(resultNpmInstall.AsSuccess.Value);
    }

    /// <summary>
    ///     Attempts to retrieve the latest version number of the "lucide-static" package from the npm registry.
    /// </summary>
    /// <returns>
    ///     A <see cref="SuccessOrFailure{TSuccess, TFailure}" /> object containing the latest version number as a success,
    ///     or an error message as a failure if the operation was unsuccessful.
    /// </returns>
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

    /// Attempts to update the version of "lucide-static" in the package.json file to the specified latest version.
    /// <param name="latestVersion">The latest version number to update the "lucide-static" dependency to.</param>
    /// <param name="args">
    ///     An instance of UpdateLucideStaticParameters containing configuration parameters for the update
    ///     process.
    /// </param>
    /// <returns>
    ///     A SuccessOrFailure object containing a success message if the update was successful, or an error message if the
    ///     operation failed.
    /// </returns>
    private async static Task<SuccessOrFailure<string, string>> TryUpdatePackageJson(string latestVersion, UpdateLucideStaticParameters args) {
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

    /// <summary>
    ///     Executes the npm install command in the specified working directory.
    /// </summary>
    /// <param name="args">The parameters containing the root directory and npm location.</param>
    /// <returns>
    ///     A Success result with a message if npm install completes successfully;
    ///     otherwise, a Failure result with an error message.
    /// </returns>
    private async static Task<SuccessOrFailure<string, string>> RunNpmInstall(UpdateLucideStaticParameters args) {
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
