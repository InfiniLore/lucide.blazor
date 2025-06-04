// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Tools.InfiniLore.Lucide.Library.Contracts;

namespace Tools.InfiniLore.Lucide.Library;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GitLibrary(ILogger<GitLibrary> logger, IUpdateLucideParameters parameters) {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task<bool> CommitChanges(string version) {
        try {
            var gitProcesses = new[] {
                new ProcessStartInfo {
                    FileName = "git",
                    Arguments = "add .",
                    WorkingDirectory = parameters.Root
                },
                new ProcessStartInfo {
                    FileName = "git",
                    Arguments = $"commit -m \"Feat: Updated Lucide to version {version}\"",
                    WorkingDirectory = parameters.Root
                }
            };

            foreach (ProcessStartInfo startInfo in gitProcesses) {
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                using var process = new Process();
                process.StartInfo = startInfo;
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0) {
                    logger.Error("Git command failed: {Command}\nError: {Error}", startInfo.Arguments, error);
                    return false;
                }

                logger.Information("Git command succeeded: {Command}\nOutput: {Output}", startInfo.Arguments, output);
            }

            logger.Information("Committed changes to git");
            return true;
        }
        catch (Exception ex) {
            logger.Error(ex, "Error executing git commands");
            return false;
        }
    }
    public async Task AutomateVersionBumpAsync(string newVersion) {
        var processStartInfo = new ProcessStartInfo {
            FileName = "dotnet",
            WorkingDirectory = parameters.Root,

            Arguments = "run --project \"src/Tools.InfiniLore.Lucide\" git-version-bump --section=\"manual\" --projects=\"%PROJECTS%\" --push --root=\"\"",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using Process? process = Process.Start(processStartInfo);
        if (process is null) throw new Exception("Could not start process");

        // Create a buffer to read output line by line
        using StreamReader outputReader = process.StandardOutput;
        using StreamReader errorReader = process.StandardError;

        // Start async reading of error stream
        Task<string> errorTask = errorReader.ReadToEndAsync();

        while (!process.HasExited) {
            string? line = await outputReader.ReadLineAsync();
            if (line == null) break;

            Console.WriteLine($"Output: {line}");// Optional: for logging/debugging

            // When it asks for version, provide the new version
            if (line.Contains("Please enter a semantic version:")) {
                await process.StandardInput.WriteLineAsync(newVersion);
                Console.WriteLine($"Provided version: {newVersion}");
            }
            // When it asks for confirmation, automatically say yes
            else if (line.Contains("Do you want to Git tag & push to origin?")) {
                await process.StandardInput.WriteLineAsync("y");
                Console.WriteLine("Automatically confirmed with 'y'");
            }
        }

        await process.WaitForExitAsync();

        // Check if there were any errors
        string errors = await errorTask;
        if (!string.IsNullOrEmpty(errors)) {
            Console.WriteLine($"Errors occurred: {errors}");
        }

        if (process.ExitCode != 0) {
            throw new Exception($"git-version-bump failed with exit code: {process.ExitCode}");
        }
    }


}
