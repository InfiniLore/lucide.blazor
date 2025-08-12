// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Library.Shared;
using DevTools.InfiniLore.Lucide.Library.Contracts;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DevTools.InfiniLore.Lucide.Library;
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
    public static async Task AutomateVersionBumpAsync(string newVersion) {
        if (!SemanticVersionDto.TryParse(newVersion, out SemanticVersionDto? dto)) throw new Exception("Invalid version");

        await GitHelpers.TryCreateGitCommit(dto);
        await GitHelpers.TryCreateGitTag(dto);
        await GitHelpers.TryPushToOrigin();
        await GitHelpers.TryPushTagsToOrigin();
    }
}
