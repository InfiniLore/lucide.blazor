// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;

namespace InfiniLore.Lucide.Tools.Commands;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class UpdateLucideStaticParameters : ICommandParameters  {
    [ArgValue("root"), Description("The root directory of the project to update")] 
    public string Root { get; set; } = "../../../../../";
    
    [ArgValue("npm"), Description("Install location of npm")]
    public string NpmLocation { get; set; } = @"C:\Program Files\nodejs\npm.cmd";
    
    public string AppendRoot(string path) => Path.Join(Root, path);

}
