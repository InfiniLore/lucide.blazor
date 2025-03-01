// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;

namespace InfiniLore.Lucide.Tools.Commands;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly partial struct UpdateLucideStaticParameters : IParameters {
    [CliArgsParameter("root", "r")]
    [CliArgsDescription("The root directory of the project to update")]
    public string Root { get; init; } = "../../../../../";

    [CliArgsParameter("npm", "n")]
    [CliArgsDescription("Install location of npm")]
    public string NpmLocation { get; init; } = @"C:\Program Files\nodejs\npm.cmd";

    public string AppendRoot(string path) => Path.Join(Root, path);
}
