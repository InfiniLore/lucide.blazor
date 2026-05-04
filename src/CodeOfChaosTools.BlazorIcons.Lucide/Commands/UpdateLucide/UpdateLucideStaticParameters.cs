// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaosTools.BlazorIcons.Lucide.Library.Contracts;

namespace CodeOfChaosTools.BlazorIcons.Lucide.Commands.UpdateLucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public record UpdateLucideStaticParameters : ICliParameters, IUpdateLucideParameters {
    [CliData("root", "r")]
    // [CliArgsDescription("The root directory of the project to update")]
    public string Root { get; init; } = "../../../../../";

    [CliData("npm", "n")]
    // [CliArgsDescription("Install location of npm")]
    public string NpmLocation { get; init; } = @"C:\Program Files\nodejs\npm.cmd";
    
    [CliData("razor-output-folder", "of")]
    // [CliArgsDescription("The root directory of the project to update")]
    public string RazorOutputFolder { get; init; } = "../../../../../src/CodeOfChaos.BlazorIcons.Lucide/Components/Icons/";
    
    [CliData("strict", "s")]
    // [CliArgsDescription("Fail on errors")]
    public bool Strict { get; init; } = false;

    public string AppendRoot(string path) => Path.Join(Root, path);
}
