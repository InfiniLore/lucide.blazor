// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using Tools.InfiniLore.Lucide.Library.Contracts;

namespace Tools.InfiniLore.Lucide.Commands.UpdateLucide;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly partial struct UpdateLucideStaticParameters : IParameters, IUpdateLucideParameters {
    [CliArgsParameter("root", "r")]
    [CliArgsDescription("The root directory of the project to update")]
    public string Root { get; init; } = "../../../../../";

    [CliArgsParameter("npm", "n")]
    [CliArgsDescription("Install location of npm")]
    public string NpmLocation { get; init; } = @"C:\Program Files\nodejs\npm.cmd";
    
    [CliArgsParameter("razor-output-folder", "of")]
    [CliArgsDescription("The root directory of the project to update")]
    public string RazorOutputFolder { get; init; } = "../../../../../src/InfiniLore.Lucide/Components/Icons/";
    
    [CliArgsParameter("strict", "s")]
    [CliArgsDescription("Fail on errors")]
    public bool Strict { get; init; } = false;

    public string AppendRoot(string path) => Path.Join(Root, path);
}
