// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Lucide.Generators.Raw.Helpers;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly struct LucideSvgFile(string name, string svg) {
    public string Name => name;
    public string PascalCaseName => name.ToPascalCase();
    public string CamelCaseName => name.ToPascalCase();
    
    public string Svg => svg;
}
