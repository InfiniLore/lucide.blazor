// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Lucide.Data;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface ILucideIconData {
    public string DirectImport { get; }
    public string DirectImportNoComments { get; }
    public string SvgContent { get; }
    public string Flat { get; }
    public string FlatNoComments { get; }
    public string FlatSvgContent { get; }
}
