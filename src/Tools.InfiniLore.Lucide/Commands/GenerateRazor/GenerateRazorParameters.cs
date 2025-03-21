// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;

namespace Tools.InfiniLore.Lucide.Commands.GenerateRazor;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly partial struct GenerateRazorParameters : IParameters {
    [CliArgsParameter("root", "r")]
    [CliArgsDescription("The root directory of the project to update")]
    public string Root { get; init; } = "../../../../../";
    
    [CliArgsParameter("output-folder", "of")]
    [CliArgsDescription("The root directory of the project to update")]
    public string OutputFolder { get; init; } = "../../../../../src/InfiniLore.Lucide/Icons/";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public string AppendRoot(string path) => Path.Join(Root, path);
}
