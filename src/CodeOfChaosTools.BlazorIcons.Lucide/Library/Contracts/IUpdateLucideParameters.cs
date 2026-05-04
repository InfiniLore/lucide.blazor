// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaosTools.BlazorIcons.Lucide.Library.Contracts;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IUpdateLucideParameters {
    public string Root { get; }
    public string NpmLocation { get; }
    public string RazorOutputFolder { get; }
    
    public string AppendRoot(string path);
}
